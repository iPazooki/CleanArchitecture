using Aspire.Hosting.Azure;
using Aspire.Hosting.JavaScript;
using Azure.Provisioning.AppContainers;
using Azure.Provisioning.PostgreSql;
using Microsoft.Extensions.Configuration;
using static CleanArchitecture.AppHost.EndpointHelpers;

namespace CleanArchitecture.AppHost.Environments;

internal static class ProductionEnvironmentExtensions
{
    private const string ContainerAppEnvironmentName = "cae";
    private const int HttpScaleConcurrentRequests = 50;

    internal static void ConfigureProductionEnvironment(this IDistributedApplicationBuilder builder)
    {
        bool useKeycloak = builder.Configuration.GetValue("UseKeycloak", false);
        bool useBrevo = builder.Configuration.GetValue("UseBrevo", false);

        builder.AddAzureContainerAppEnvironment(ContainerAppEnvironmentName);

        IResourceBuilder<AzureApplicationInsightsResource> applicationInsights = builder.AddAzureApplicationInsights("applicationInsights");

        IResourceBuilder<AzureKeyVaultResource> keyVault = builder.AddAzureKeyVault("keyvault");

        IResourceBuilder<ParameterResource> nextAuthSecret = builder.AddParameter("nextAuthSecret", secret: true);

        keyVault.AddSecret("kv-nextAuthSecret", nextAuthSecret);


        IResourceBuilder<ParameterResource> postgresUsername = builder.AddParameter("PostgresUsername");
        IResourceBuilder<ParameterResource> postgresPassword = builder.AddParameter("PostgresPassword", secret: true);

        keyVault.AddSecret("kv-postgresPassword", postgresPassword);


        IResourceBuilder<AzurePostgresFlexibleServerResource> postgres = builder.AddAzurePostgresFlexibleServer("postgres")
            .WithPasswordAuthentication(postgresUsername, postgresPassword)
            .ConfigureInfrastructure(infra =>
            {
                PostgreSqlFlexibleServer server = infra.GetProvisionableResources().OfType<PostgreSqlFlexibleServer>().Single();
                server.Sku = new PostgreSqlFlexibleServerSku
                {
                    Name = "Standard_B1ms",
                    Tier = PostgreSqlFlexibleServerSkuTier.Burstable
                };
                server.HighAvailability = new PostgreSqlFlexibleServerHighAvailability
                {
                    Mode = PostgreSqlFlexibleServerHighAvailabilityMode.Disabled
                };
                server.Backup = new PostgreSqlFlexibleServerBackupProperties
                {
                    BackupRetentionDays = 7,
                    GeoRedundantBackup = PostgreSqlFlexibleServerGeoRedundantBackupEnum.Disabled
                };
            });

        IResourceBuilder<AzurePostgresFlexibleServerDatabaseResource> appDb = postgres.AddDatabase(ResourceNames.PostgresDatabase, "mrpaneldb");

#pragma warning disable ASPIREAZURE002 // PublishAsAzureContainerAppJob is evaluation-only in 13.2.4
        builder.AddProject<Projects.CleanArchitecture_DbMigrator>(ResourceNames.DbMigrator)
            .WithReference(appDb)
            .WaitFor(appDb)
            .PublishAsAzureContainerAppJob((_, job) =>
            {
                job.Configuration.TriggerType = ContainerAppJobTriggerType.Manual;
                job.Configuration.ReplicaRetryLimit = 3;
                job.Configuration.ReplicaTimeout = 600;
                ApplyJobResources(job, cpu: 0.25, memory: "0.5Gi");
            });
#pragma warning restore ASPIREAZURE002

        IResourceBuilder<ProjectResource> apiProject = builder.AddProject<Projects.CleanArchitecture_Api>(ResourceNames.Api)
            .WithReference(appDb)
            .WaitFor(appDb)
            .WithReference(applicationInsights)
            .WithReference(keyVault)
            .PublishAsAzureContainerApp((_, app) =>
            {
                app.Configuration.Ingress.External = false;
                app.Configuration.Ingress.AllowInsecure = true;
                ConfigureScaleToZero(app, maxReplicas: 2);
                ApplyContainerResources(app, cpu: 0.5, memory: "1.0Gi");
            });

        EndpointReference apiInternalEndpoint = apiProject.GetEndpoint("http");

        IResourceBuilder<NodeAppResource> adminApp = builder.AddNodeApp(ResourceNames.Admin, AppHostConstants.AdminAppRelativePath, AppHostConstants.NextJsEntryPoint)
            .WithHttpEndpoint(targetPort: AppHostConstants.AdminTargetPort)
            .WithExternalHttpEndpoints()
            .WithArgs("start")
            .WithPnpm()
            .WithReference(apiProject)
            .WaitFor(apiProject)
            .WithReference(keyVault)
            .WithEnvironment("NODE_ENV", "production")
            .WithEnvironment("NEXTAUTH_SECRET", nextAuthSecret)
            .PublishAsDockerFile()
            .PublishAsAzureContainerApp((_, app) =>
            {
                ConfigureScaleToZero(app, maxReplicas: 2);
                ApplyContainerResources(app, cpu: 0.5, memory: "1.0Gi");
            });

        EndpointReference adminPublicEndpoint = adminApp.GetEndpoint("http");

        if (useKeycloak)
        {
            ConfigureKeycloak(builder, keyVault, postgres, apiProject, adminApp, apiInternalEndpoint, adminPublicEndpoint);
        }
        else
        {
            ConfigureEntra(builder, keyVault, apiProject, adminApp, apiInternalEndpoint, adminPublicEndpoint);
        }

        if (useBrevo)
        {
            ConfigureBrevo(builder, keyVault, apiProject);
        }
    }

    /// <summary>
    /// Wires the Brevo transactional email provider. Opt-in via the <c>UseBrevo</c> setting:
    /// without it the API falls back to a null email provider rather than demanding an API key.
    /// </summary>
    private static void ConfigureBrevo(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<AzureKeyVaultResource> keyVault,
        IResourceBuilder<ProjectResource> apiProject)
    {
        IResourceBuilder<ParameterResource> brevoApiKey = builder.AddParameter("brevoApiKey", secret: true);
        IResourceBuilder<ParameterResource> brevoSenderName = builder.AddParameter("brevoSenderName");
        IResourceBuilder<ParameterResource> brevoSenderEmail = builder.AddParameter("brevoSenderEmail");

        keyVault.AddSecret("kv-brevoApiKey", brevoApiKey);

        apiProject
            .WithEnvironment("Brevo__ApiKey", brevoApiKey)
            .WithEnvironment("Brevo__SenderName", brevoSenderName)
            .WithEnvironment("Brevo__SenderEmail", brevoSenderEmail);
    }

    private static void ConfigureKeycloak(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<AzureKeyVaultResource> keyVault,
        IResourceBuilder<AzurePostgresFlexibleServerResource> postgres,
        IResourceBuilder<ProjectResource> apiProject,
        IResourceBuilder<NodeAppResource> adminApp,
        EndpointReference apiInternalEndpoint,
        EndpointReference adminPublicEndpoint)
    {
        IResourceBuilder<AzurePostgresFlexibleServerDatabaseResource> keycloakDb = postgres.AddDatabase("keycloakResource", "keycloakdb");

        IResourceBuilder<ParameterResource> keycloakClientId = builder.AddParameter("keycloakClientId");
        IResourceBuilder<ParameterResource> keycloakClientSecret = builder.AddParameter("keycloakClientSecret", secret: true);
        IResourceBuilder<ParameterResource> keycloakRealm = builder.AddParameter("keycloakRealm");
        IResourceBuilder<ParameterResource> keycloakAdminUsername = builder.AddParameter("keycloakAdminUsername");
        IResourceBuilder<ParameterResource> keycloakAdminPassword = builder.AddParameter("keycloakAdminPassword", secret: true);

        IResourceBuilder<ParameterResource> keycloakDbUsername = builder.AddParameter("keycloakDbUsername");
        IResourceBuilder<ParameterResource> keycloakDbPassword = builder.AddParameter("keycloakDbPassword", secret: true);

        keyVault.AddSecret("kv-keycloakClientSecret", keycloakClientSecret);
        keyVault.AddSecret("kv-keycloakAdminPassword", keycloakAdminPassword);
        keyVault.AddSecret("kv-keycloakDbPassword", keycloakDbPassword);

        IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", AppHostConstants.KeycloakPort, keycloakAdminUsername, keycloakAdminPassword)
            .WithEnvironment("KC_DB", "postgres")
            .WithEnvironment("KC_DB_URL_DATABASE", keycloakDb.Resource.DatabaseName)
            .WithEndpoint("http", endpoint => endpoint.IsExternal = true, createIfNotExists: false)
            .WaitFor(keycloakDb)
            .PublishAsAzureContainerApp((_, app) =>
            {
                app.Template.Scale.MinReplicas = 1;
                app.Template.Scale.MaxReplicas = 2;
                ApplyContainerResources(app, cpu: 0.5, memory: "1.0Gi");
            });

        EndpointReference keycloakPublicEndpoint = keycloak.GetEndpoint("http");

        keycloak
            .WithEnvironment("KC_DB_URL_HOST", postgres.GetEndpoint("tcp").Property(EndpointProperty.Host))
            .WithEnvironment("KC_DB_URL_PORT", postgres.GetEndpoint("tcp").Property(EndpointProperty.Port))
            .WithEnvironment("KC_DB_USERNAME", keycloakDbUsername)
            .WithEnvironment("KC_DB_PASSWORD", keycloakDbPassword)
            .WithEnvironment("KC_HOSTNAME", keycloakPublicEndpoint.Property(EndpointProperty.Host))
            .WithEnvironment("KC_PROXY_HEADERS", "xforwarded")
            .WithEnvironment("KC_HTTP_ENABLED", "true");

        apiProject.WithReference(keycloak).WaitFor(keycloak);

        ReferenceExpression publicAuthUrl = BuildExternalHttpsUrl(keycloakPublicEndpoint);
        ReferenceExpression publicIssuer = ReferenceExpression.Create($"{publicAuthUrl}/realms/{keycloakRealm.Resource}");

        apiProject
            .WithEnvironment("Authentication__Provider", "Keycloak")
            .WithEnvironment("Keycloak__AuthorizationUrl", ReferenceExpression.Create($"{publicIssuer}/protocol/openid-connect/auth"))
            .WithEnvironment("Keycloak__TokenUrl", ReferenceExpression.Create($"{publicIssuer}/protocol/openid-connect/token"))
            .WithEnvironment("Keycloak__ValidIssuers", publicIssuer)
            .WithEnvironment("Keycloak__RequireHttpsMetadata", "false")
            .WithEnvironment("Keycloak__Realm", keycloakRealm)
            .WithEnvironment("Keycloak__Audience", keycloakClientId)
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production");

        ReferenceExpression adminUrl = BuildExternalHttpsUrl(adminPublicEndpoint);
        ReferenceExpression apiUrl = BuildInternalHttpUrl(apiInternalEndpoint);
        ReferenceExpression keycloakIssuer = ReferenceExpression.Create($"{BuildExternalHttpsUrl(keycloakPublicEndpoint)}/realms/{keycloakRealm.Resource}");

        adminApp
            .WithEnvironment("AUTH_PROVIDER", "Keycloak")
            .WithEnvironment("NEXTAUTH_URL", adminUrl)
            .WithEnvironment("API_BASE_URL", apiUrl)
            .WithEnvironment("KEYCLOAK_CLIENT_ID", keycloakClientId)
            .WithEnvironment("KEYCLOAK_CLIENT_SECRET", keycloakClientSecret)
            .WithEnvironment("KEYCLOAK_ISSUER", keycloakIssuer);
    }

    private static void ConfigureEntra(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<AzureKeyVaultResource> keyVault,
        IResourceBuilder<ProjectResource> apiProject,
        IResourceBuilder<NodeAppResource> adminApp,
        EndpointReference apiInternalEndpoint,
        EndpointReference adminPublicEndpoint)
    {
        IResourceBuilder<ParameterResource> entraApiInstance = builder.AddParameter("EntraAPIInstance");
        IResourceBuilder<ParameterResource> entraApiDomain = builder.AddParameter("EntraAPIPrimaryDomain");
        IResourceBuilder<ParameterResource> entraApiClientId = builder.AddParameter("EntraAPIClientId");
        IResourceBuilder<ParameterResource> entraApiAudienceId = builder.AddParameter("EntraAPIAudienceId");

        IResourceBuilder<ParameterResource> entraTenantId = builder.AddParameter("EntraTenantId");

        IResourceBuilder<ParameterResource> entraAdminClientId = builder.AddParameter("EntraAdminClientId");
        IResourceBuilder<ParameterResource> entraAdminClientSecret = builder.AddParameter("EntraAdminClientSecret", secret: true);
        IResourceBuilder<ParameterResource> entraAdminScope = builder.AddParameter("EntraAdminScope");
        IResourceBuilder<ParameterResource> entraAdminOpenId = builder.AddParameter("EntraAdminOpenIdURL");

        keyVault.AddSecret("kv-entraAdminClientSecret", entraAdminClientSecret);

        apiProject
            .WithEnvironment("Authentication__Provider", "Entra")
            .WithEnvironment("AzureAd__Instance", entraApiInstance)
            .WithEnvironment("AzureAd__Domain", entraApiDomain)
            .WithEnvironment("AzureAd__TenantId", entraTenantId)
            .WithEnvironment("AzureAd__ClientId", entraApiClientId)
            .WithEnvironment("AzureAd__Audience", entraApiAudienceId)
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production");

        ReferenceExpression adminUrl = BuildExternalHttpsUrl(adminPublicEndpoint);
        ReferenceExpression apiUrl = BuildInternalHttpUrl(apiInternalEndpoint);

        adminApp
            .WithEnvironment("AUTH_PROVIDER", "Entra")
            .WithEnvironment("NEXTAUTH_URL", adminUrl)
            .WithEnvironment("API_BASE_URL", apiUrl)
            .WithEnvironment("ENTRA_CLIENT_ID", entraAdminClientId)
            .WithEnvironment("ENTRA_CLIENT_SECRET", entraAdminClientSecret)
            .WithEnvironment("ENTRA_TENANT_ID", entraTenantId)
            .WithEnvironment("ENTRA_SCOPES", entraAdminScope)
            .WithEnvironment("ENTRA_OPENID_CONNECT", entraAdminOpenId);
    }

    private static void ConfigureScaleToZero(ContainerApp app, int maxReplicas)
    {
        app.Template.Scale.MinReplicas = 0;
        app.Template.Scale.MaxReplicas = maxReplicas;
        app.Template.Scale.Rules.Add(new ContainerAppScaleRule
        {
            Name = "http",
            Http = new ContainerAppHttpScaleRule
            {
                Metadata =
                {
                    ["concurrentRequests"] = HttpScaleConcurrentRequests.ToString(System.Globalization.CultureInfo.InvariantCulture)
                }
            }
        });
    }

    private static void ApplyContainerResources(ContainerApp app, double cpu, string memory)
    {
        ContainerAppContainer primary = app.Template.Containers[0].Value!;
        primary.Resources = new AppContainerResources { Cpu = cpu, Memory = memory };
    }

    private static void ApplyJobResources(ContainerAppJob job, double cpu, string memory)
    {
        ContainerAppContainer primary = job.Template.Containers[0].Value!;
        primary.Resources = new AppContainerResources { Cpu = cpu, Memory = memory };
    }
}
