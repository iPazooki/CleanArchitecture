using Aspire.Hosting.Azure;
using Aspire.Hosting.JavaScript;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

const string TestingEnvironment = "Testing";
const string AdminAppName = "admin";
const string ApiProjectName = "cleanarchitecture-api";
const string PostgresDatabaseResourceName = "postgresDatabaseResource";

const int AdminHostPort = 65499;   // Pinned for stable local dev URL
const int AdminTargetPort = 3000;  // Next.js internal port
const int KeycloakPort = 8080;     // Pinned for stable auth callback/local access

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment(TestingEnvironment))
{
    ConfigureTestingEnvironment(builder);
}
else if (builder.Environment.IsDevelopment() && builder.ExecutionContext.IsRunMode)
{
    ConfigureDevelopmentEnvironment(builder);
}
else
{
    ConfigureProductionEnvironment(builder);
}

await builder.Build().RunAsync().ConfigureAwait(false);

static void ConfigureTestingEnvironment(IDistributedApplicationBuilder builder)
{
    IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("testingPostgres");

    IResourceBuilder<PostgresDatabaseResource> postgresdb =
        postgres.AddDatabase(PostgresDatabaseResourceName, "testingDb");

    IResourceBuilder<ProjectResource> migrator = builder.AddProject<Projects.CleanArchitecture_DbMigrator>("db-migrator")
        .WithReference(postgresdb)
        .WaitFor(postgresdb)
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", TestingEnvironment);

    IResourceBuilder<ProjectResource> project = builder.AddProject<Projects.CleanArchitecture_Api>(ApiProjectName)
        .WithReference(postgresdb)
        .WaitFor(postgresdb)
        .WaitFor(migrator);

    project.WithEnvironment("ASPNETCORE_ENVIRONMENT", TestingEnvironment);
}

static void ConfigureDevelopmentEnvironment(IDistributedApplicationBuilder builder)
{
    IResourceBuilder<PostgresDatabaseResource> postgres = builder.AddPostgres("postgres")
        .WithDataVolume("postgres_data")
        .WithPgAdmin()
        .WithLifetime(ContainerLifetime.Persistent)
        .AddDatabase(PostgresDatabaseResourceName, "mrpaneldb");

    IResourceBuilder<ParameterResource> username =
        builder.AddParameter("keycloakAdminUsername", "admin");

    IResourceBuilder<ParameterResource> password =
        builder.AddParameter("keycloakAdminPassword", "admin", secret: true);

    IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", KeycloakPort, username, password)
        .WithRealmImport("./Realms")
        .WithDataVolume()
        .WithOtlpExporter()
        .WithLifetime(ContainerLifetime.Persistent);

    IResourceBuilder<ProjectResource> migrator = builder.AddProject<Projects.CleanArchitecture_DbMigrator>("db-migrator")
        .WithReference(postgres)
        .WaitFor(postgres);

    IResourceBuilder<ProjectResource> apiProject = builder.AddProject<Projects.CleanArchitecture_Api>(ApiProjectName)
        .WithReference(postgres)
        .WaitFor(postgres)
        .WaitFor(migrator)
        .WithReference(keycloak)
        .WaitFor(keycloak);

    builder.AddNodeApp(AdminAppName, "../../CleanArchitecture.Presentation/admin", "node_modules/next/dist/bin/next")
        .WithHttpEndpoint(targetPort: AdminTargetPort, port: AdminHostPort)
        .WithArgs("dev")
        .WithPnpm()
        .WithReference(apiProject)
        .WaitFor(apiProject);
}

static void ConfigureProductionEnvironment(IDistributedApplicationBuilder builder)
{
    bool useKeycloak = builder.Configuration.GetValue<bool>("UseKeycloak", false);

    IResourceBuilder<AzureApplicationInsightsResource> applicationInsights = builder.AddAzureApplicationInsights("applicationInsights");

    IResourceBuilder<AzureKeyVaultResource> keyVault = builder.AddAzureKeyVault("keyvault");

    IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("postgres")
        .WithDataVolume("postgres_data")
        .WithLifetime(ContainerLifetime.Persistent);

    IResourceBuilder<PostgresDatabaseResource> appDb = postgres.AddDatabase(PostgresDatabaseResourceName, "mrpaneldb");

    IResourceBuilder<ProjectResource> migrator = builder.AddProject<Projects.CleanArchitecture_DbMigrator>("db-migrator")
        .WithReference(appDb)
        .WaitFor(appDb);

    IResourceBuilder<ParameterResource> nextAuthSecret = builder.AddParameter("nextAuthSecret", secret: true);

    IResourceBuilder<ProjectResource> apiProject = builder.AddProject<Projects.CleanArchitecture_Api>(ApiProjectName)
        .WithReference(appDb)
        .WaitFor(appDb)
        .WaitFor(migrator)
        .WithReference(applicationInsights)
        .WithReference(keyVault);

    EndpointReference apiInternalEndpoint = apiProject.GetEndpoint("http");

    IResourceBuilder<NodeAppResource> adminApp = builder.AddNodeApp(AdminAppName, "../../CleanArchitecture.Presentation/admin", "node_modules/next/dist/bin/next")
        .WithHttpEndpoint(targetPort: AdminTargetPort)
        .WithExternalHttpEndpoints()
        .WithArgs("start")
        .WithPnpm()
        .WithReference(apiProject)
        .WaitFor(apiProject)
        .WithEnvironment("NODE_ENV", "production")
        .WithEnvironment("NEXTAUTH_SECRET", nextAuthSecret);

    EndpointReference adminPublicEndpoint = adminApp.GetEndpoint("http");

    if (useKeycloak)
    {
        IResourceBuilder<PostgresDatabaseResource> keycloakDb = postgres.AddDatabase("keycloakResource", "keycloakdb");

        IResourceBuilder<ParameterResource> keycloakClientId = builder.AddParameter("keycloakClientId");
        IResourceBuilder<ParameterResource> keycloakClientSecret = builder.AddParameter("keycloakClientSecret", secret: true);
        IResourceBuilder<ParameterResource> keycloakRealm = builder.AddParameter("keycloakRealm");
        IResourceBuilder<ParameterResource> keycloakAdminUsername = builder.AddParameter("keycloakAdminUsername");
        IResourceBuilder<ParameterResource> keycloakAdminPassword = builder.AddParameter("keycloakAdminPassword", secret: true);

        IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", KeycloakPort, keycloakAdminUsername, keycloakAdminPassword)
            .WithEnvironment("KC_DB", "postgres")
            .WithEnvironment("KC_DB_URL_DATABASE", keycloakDb.Resource.DatabaseName)
            .WithEndpoint("http", endpoint => endpoint.IsExternal = true, createIfNotExists: false)
            .WaitFor(keycloakDb);

        EndpointReference keycloakPublicEndpoint = keycloak.GetEndpoint("http");

        keycloak.WithEnvironment(context =>
        {
            context.EnvironmentVariables["KC_DB_URL_HOST"] = postgres.GetEndpoint("tcp").Property(EndpointProperty.Host).ValueExpression;
            context.EnvironmentVariables["KC_DB_URL_PORT"] = postgres.GetEndpoint("tcp").Property(EndpointProperty.Port).ValueExpression;
            context.EnvironmentVariables["KC_DB_USERNAME"] = postgres.Resource.UserNameParameter!.ValueExpression;
            context.EnvironmentVariables["KC_DB_PASSWORD"] = postgres.Resource.PasswordParameter!.ValueExpression;
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
            context.EnvironmentVariables["KEYCLOAK_CLIENT_ID"] = keycloakClientId;
            context.EnvironmentVariables["KEYCLOAK_CLIENT_SECRET"] = keycloakClientSecret;
            context.EnvironmentVariables["KEYCLOAK_ISSUER"] = keycloakIssuer;
        });
    }
    else
    {
        IResourceBuilder<ParameterResource> entraClientId = builder.AddParameter("entraClientId");
        IResourceBuilder<ParameterResource> entraClientSecret = builder.AddParameter("entraClientSecret", secret: true);
        IResourceBuilder<ParameterResource> entraTenantId = builder.AddParameter("entraTenantId");

        apiProject.WithEnvironment(context =>
        {
            context.EnvironmentVariables["Authentication__Provider"] = "Entra";
            context.EnvironmentVariables["AzureAd__Instance"] = "https://login.microsoftonline.com/";
            context.EnvironmentVariables["AzureAd__TenantId"] = entraTenantId.Resource.ValueExpression;
            context.EnvironmentVariables["AzureAd__ClientId"] = entraClientId.Resource.ValueExpression;
            context.EnvironmentVariables["AzureAd__Audience"] = entraClientId.Resource.ValueExpression;
            context.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Production";
        });

        adminApp.WithEnvironment(context =>
        {
            string adminUrl = BuildExternalHttpsUrl(adminPublicEndpoint);
            string apiUrl = BuildInternalHttpUrl(apiInternalEndpoint);

            context.EnvironmentVariables["AUTH_PROVIDER"] = "Entra";
            context.EnvironmentVariables["NEXTAUTH_URL"] = adminUrl;
            context.EnvironmentVariables["API_BASE_URL"] = apiUrl;
            context.EnvironmentVariables["ENTRA_CLIENT_ID"] = entraClientId;
            context.EnvironmentVariables["ENTRA_CLIENT_SECRET"] = entraClientSecret;
            context.EnvironmentVariables["ENTRA_TENANT_ID"] = entraTenantId;
            context.EnvironmentVariables["ENTRA_SCOPES"] = $"api://{entraClientId.Resource.ValueExpression}/.default";
            context.EnvironmentVariables["ENTRA_OPENID_CONNECT"] = $"https://login.microsoftonline.com/{entraTenantId.Resource.ValueExpression}/v2.0/.well-known/openid-configuration";
        });
    }
}

static string BuildExternalHttpsUrl(EndpointReference endpoint)
{
    return $"https://{endpoint.Property(EndpointProperty.Host).ValueExpression}";
}

static string BuildInternalHttpUrl(EndpointReference endpoint)
{
    return $"http://{endpoint.Property(EndpointProperty.Host).ValueExpression}:{endpoint.Property(EndpointProperty.Port).ValueExpression}";
}
