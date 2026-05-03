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

        builder.AddAzureContainerAppEnvironment(ContainerAppEnvironmentName);

        IResourceBuilder<AzureApplicationInsightsResource> applicationInsights = builder.AddAzureApplicationInsights("applicationInsights");

        IResourceBuilder<AzureKeyVaultResource> keyVault = builder.AddAzureKeyVault("keyvault");

        IResourceBuilder<ParameterResource> nextAuthSecret = builder.AddParameter("nextAuthSecret", secret: true);

        keyVault.AddSecret("kv-nextAuthSecret", nextAuthSecret);

        IResourceBuilder<AzurePostgresFlexibleServerResource> postgres = builder.AddAzurePostgresFlexibleServer("postgres")
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
                server.StorageSizeInGB = 32;
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

        keycloak.WithEnvironment(context =>
        {
            context.EnvironmentVariables["KC_DB_URL_HOST"] = postgres.GetEndpoint("tcp").Property(EndpointProperty.Host).ValueExpression;
            context.EnvironmentVariables["KC_DB_URL_PORT"] = postgres.GetEndpoint("tcp").Property(EndpointProperty.Port).ValueExpression;
            context.EnvironmentVariables["KC_DB_USERNAME"] = keycloakDbUsername.Resource.ValueExpression;
            context.EnvironmentVariables["KC_DB_PASSWORD"] = keycloakDbPassword.Resource.ValueExpression;
            context.EnvironmentVariables["KC_HOSTNAME"] = keycloakPublicEndpoint.Property(EndpointProperty.Host).ValueExpression;
            context.EnvironmentVariables["KC_PROXY_HEADERS"] = "xforwarded";
            context.EnvironmentVariables["KC_HTTP_ENABLED"] = "true";
        });

        apiProject.WithReference(keycloak).WaitFor(keycloak);

        apiProject.WithEnvironment(context =>
        {
            string publicAuthUrl = BuildExternalHttpsUrl(keycloakPublicEndpoint);
            string realm = keycloakRealm.Resource.ValueExpression;
            string audience = keycloakClientId.Resource.ValueExpression;
            string publicIssuer = $"{publicAuthUrl}/realms/{realm}";

            context.EnvironmentVariables["Authentication__Provider"] = "Keycloak";
            context.EnvironmentVariables["Keycloak__AuthorizationUrl"] = $"{publicIssuer}/protocol/openid-connect/auth";
            context.EnvironmentVariables["Keycloak__TokenUrl"] = $"{publicIssuer}/protocol/openid-connect/token";
            context.EnvironmentVariables["Keycloak__ValidIssuers"] = publicIssuer;
            context.EnvironmentVariables["Keycloak__RequireHttpsMetadata"] = "false";
            context.EnvironmentVariables["Keycloak__Realm"] = realm;
            context.EnvironmentVariables["Keycloak__Audience"] = audience;
            context.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Production";
        });

        adminApp.WithEnvironment(context =>
        {
            string adminUrl = BuildExternalHttpsUrl(adminPublicEndpoint);
            string apiUrl = BuildInternalHttpUrl(apiInternalEndpoint);
            string keycloakIssuer = $"{BuildExternalHttpsUrl(keycloakPublicEndpoint)}/realms/{keycloakRealm.Resource.ValueExpression}";

            context.EnvironmentVariables["AUTH_PROVIDER"] = "Keycloak";
            context.EnvironmentVariables["NEXTAUTH_URL"] = adminUrl;
            context.EnvironmentVariables["API_BASE_URL"] = apiUrl;
            context.EnvironmentVariables["KEYCLOAK_CLIENT_ID"] = keycloakClientId.Resource.ValueExpression;
            context.EnvironmentVariables["KEYCLOAK_CLIENT_SECRET"] = keycloakClientSecret.Resource.ValueExpression;
            context.EnvironmentVariables["KEYCLOAK_ISSUER"] = keycloakIssuer;
        });
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


        apiProject.WithEnvironment(context =>
        {
            context.EnvironmentVariables["Authentication__Provider"] = "Entra";
            context.EnvironmentVariables["AzureAd__Instance"] = entraApiInstance.Resource.ValueExpression;
            context.EnvironmentVariables["AzureAd__Domain"] = entraApiDomain.Resource.ValueExpression;
            context.EnvironmentVariables["AzureAd__TenantId"] = entraTenantId.Resource.ValueExpression;
            context.EnvironmentVariables["AzureAd__ClientId"] = entraApiClientId.Resource.ValueExpression;
            context.EnvironmentVariables["AzureAd__Audience"] = entraApiAudienceId.Resource.ValueExpression;
            context.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Production";
        });

        adminApp.WithEnvironment(context =>
        {
            string adminUrl = BuildExternalHttpsUrl(adminPublicEndpoint);
            string apiUrl = BuildInternalHttpUrl(apiInternalEndpoint);

            context.EnvironmentVariables["AUTH_PROVIDER"] = "Entra";
            context.EnvironmentVariables["NEXTAUTH_URL"] = adminUrl;
            context.EnvironmentVariables["API_BASE_URL"] = apiUrl;
            context.EnvironmentVariables["ENTRA_CLIENT_ID"] = entraAdminClientId.Resource.ValueExpression;
            context.EnvironmentVariables["ENTRA_CLIENT_SECRET"] = entraAdminClientSecret.Resource.ValueExpression;
            context.EnvironmentVariables["ENTRA_TENANT_ID"] = entraTenantId.Resource.ValueExpression;
            context.EnvironmentVariables["ENTRA_SCOPES"] = entraAdminScope.Resource.ValueExpression;
            context.EnvironmentVariables["ENTRA_OPENID_CONNECT"] = entraAdminOpenId.Resource.ValueExpression;
        });
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
