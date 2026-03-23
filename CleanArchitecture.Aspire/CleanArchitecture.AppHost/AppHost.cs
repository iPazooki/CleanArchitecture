using Aspire.Hosting.Azure;
using Aspire.Hosting.JavaScript;
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

    IResourceBuilder<ProjectResource> project = builder.AddProject<Projects.CleanArchitecture_Api>(ApiProjectName)
        .WithReference(postgresdb)
        .WaitFor(postgresdb);

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

    IResourceBuilder<ProjectResource> apiProject = builder.AddProject<Projects.CleanArchitecture_Api>(ApiProjectName)
        .WithReference(postgres)
        .WaitFor(postgres)
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
    IResourceBuilder<AzureApplicationInsightsResource> applicationInsights = builder.AddAzureApplicationInsights("applicationInsights");

    IResourceBuilder<AzureKeyVaultResource> keyVault = builder.AddAzureKeyVault("keyvault");

    // Azure PostgreSQL Flexible Server with password auth (credentials stored in Key Vault)
    IResourceBuilder<AzurePostgresFlexibleServerResource> postgres = builder.AddAzurePostgresFlexibleServer("postgres")
        .WithPasswordAuthentication(keyVault);

    IResourceBuilder<AzurePostgresFlexibleServerDatabaseResource> appDb = postgres.AddDatabase(PostgresDatabaseResourceName, "mrpaneldb");
    IResourceBuilder<AzurePostgresFlexibleServerDatabaseResource> keycloakDb = postgres.AddDatabase("keycloakResource", "keycloakdb");

    // Public URLs are derived from ACA external endpoints; only secrets and non-endpoint settings are supplied at deployment time.
    IResourceBuilder<ParameterResource> nextAuthSecret = builder.AddParameter("nextAuthSecret", secret: true);
    IResourceBuilder<ParameterResource> keycloakClientId = builder.AddParameter("keycloakClientId");
    IResourceBuilder<ParameterResource> keycloakClientSecret = builder.AddParameter("keycloakClientSecret", secret: true);
    IResourceBuilder<ParameterResource> keycloakRealm = builder.AddParameter("keycloakRealm");
    IResourceBuilder<ParameterResource> keycloakAudience = builder.AddParameter("keycloakAudience");

    // Keycloak admin credentials via Aspire parameters (set at deployment time)
    IResourceBuilder<ParameterResource> keycloakAdminUsername = builder.AddParameter("keycloakAdminUsername");
    IResourceBuilder<ParameterResource> keycloakAdminPassword = builder.AddParameter("keycloakAdminPassword", secret: true);

    // Keycloak on ACA, backed by Azure PostgreSQL via KC_DB environment variables
    IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", KeycloakPort, keycloakAdminUsername, keycloakAdminPassword)
        .WithEnvironment("KC_DB", "postgres")
        .WithEnvironment("KC_DB_URL_DATABASE", keycloakDb.Resource.DatabaseName)
        .WithEndpoint("http", endpoint => endpoint.IsExternal = true, createIfNotExists: false)
        .WaitFor(keycloakDb);

    EndpointReference keycloakPublicEndpoint = keycloak.GetEndpoint("http");

    keycloak.WithEnvironment(context =>
    {
        context.EnvironmentVariables["KC_DB_URL_HOST"] = postgres.Resource.HostName;
        context.EnvironmentVariables["KC_DB_URL_PORT"] = "5432";
        context.EnvironmentVariables["KC_DB_USERNAME"] = postgres.Resource.UserName!.ValueExpression;
        context.EnvironmentVariables["KC_DB_PASSWORD"] = postgres.Resource.Password!.ValueExpression;
        context.EnvironmentVariables["KC_HOSTNAME"] = keycloakPublicEndpoint.Property(EndpointProperty.Host).ValueExpression;
        context.EnvironmentVariables["KC_PROXY_HEADERS"] = "xforwarded";
        context.EnvironmentVariables["KC_HTTP_ENABLED"] = "true";
    });

    IResourceBuilder<ProjectResource> apiProject = builder.AddProject<Projects.CleanArchitecture_Api>(ApiProjectName)
        .WithReference(appDb)
        .WaitFor(appDb)
        .WithReference(keycloak)
        .WaitFor(keycloak)
        .WithExternalHttpEndpoints()
        .WithReference(applicationInsights)
        .WithReference(keyVault);

    EndpointReference apiPublicEndpoint = apiProject.GetEndpoint("https");

    apiProject.WithEnvironment(context =>
    {
        string authUrl = BuildExternalHttpsUrl(keycloakPublicEndpoint);
        string realm = keycloakRealm.Resource.ValueExpression;
        string audience = keycloakAudience.Resource.ValueExpression;
        string issuer = $"{authUrl}/realms/{realm}";

        context.EnvironmentVariables["Keycloak__AuthorizationUrl"] = $"{issuer}/protocol/openid-connect/auth";
        context.EnvironmentVariables["Keycloak__TokenUrl"] = $"{issuer}/protocol/openid-connect/token";
        context.EnvironmentVariables["Keycloak__ValidIssuers"] = issuer;
        context.EnvironmentVariables["Keycloak__Realm"] = realm;
        context.EnvironmentVariables["Keycloak__Audience"] = audience;
        context.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Production";
    });

    IResourceBuilder<NodeAppResource> adminApp = builder.AddNodeApp(AdminAppName, "../../CleanArchitecture.Presentation/admin", "node_modules/next/dist/bin/next")
        .WithHttpEndpoint(targetPort: AdminTargetPort)
        .WithExternalHttpEndpoints()
        .WithArgs("start")
        .WithPnpm()
        .WithReference(apiProject)
        .WaitFor(apiProject)
        .WithEnvironment("NODE_ENV", "production")
        .WithEnvironment("NEXTAUTH_SECRET", nextAuthSecret)
        .WithEnvironment("KEYCLOAK_CLIENT_ID", keycloakClientId)
        .WithEnvironment("KEYCLOAK_CLIENT_SECRET", keycloakClientSecret);

    EndpointReference adminPublicEndpoint = adminApp.GetEndpoint("http");

    adminApp.WithEnvironment(context =>
    {
        string adminUrl = BuildExternalHttpsUrl(adminPublicEndpoint);
        string apiUrl = BuildExternalHttpsUrl(apiPublicEndpoint);
        string keycloakIssuer = $"{BuildExternalHttpsUrl(keycloakPublicEndpoint)}/realms/{keycloakRealm.Resource.ValueExpression}";

        context.EnvironmentVariables["NEXTAUTH_URL"] = adminUrl;
        context.EnvironmentVariables["API_BASE_URL"] = apiUrl;
        context.EnvironmentVariables["KEYCLOAK_ISSUER"] = keycloakIssuer;
    });
}

static string BuildExternalHttpsUrl(EndpointReference endpoint)
{
    return $"https://{endpoint.Property(EndpointProperty.Host).ValueExpression}";
}
