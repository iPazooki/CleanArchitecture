using Aspire.Hosting.Azure;
using Microsoft.Extensions.Hosting;

const string TestingEnvironment = "Testing";
const string AdminAppName = "admin";
const string ApiProjectName = "cleanarchitecture-api";
const string PostgresDatabaseResourceName = "mrpanel";

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
    IResourceBuilder<AzureKeyVaultResource> keyVault = builder.AddAzureKeyVault("keyvault");

    // Azure PostgreSQL Flexible Server with password auth (credentials stored in Key Vault)
    IResourceBuilder<AzurePostgresFlexibleServerResource> postgres = builder.AddAzurePostgresFlexibleServer("postgres")
        .WithPasswordAuthentication(keyVault);

    IResourceBuilder<AzurePostgresFlexibleServerDatabaseResource> appDb = postgres.AddDatabase(PostgresDatabaseResourceName, "mrpaneldb");
    IResourceBuilder<AzurePostgresFlexibleServerDatabaseResource> keycloakDb = postgres.AddDatabase("keycloakResource", "keycloakdb");

    // Keycloak admin credentials via Aspire parameters (set at deployment time)
    IResourceBuilder<ParameterResource> keycloakAdminUsername = builder.AddParameter("keycloakAdminUsername");

    IResourceBuilder<ParameterResource> keycloakAdminPassword = builder.AddParameter("keycloakAdminPassword", secret: true);

    // Keycloak on ACA, backed by Azure PostgreSQL via KC_DB environment variables
    IResourceBuilder<KeycloakResource> keycloak =
        builder.AddKeycloak("keycloak", KeycloakPort, keycloakAdminUsername, keycloakAdminPassword)
            .WithEnvironment("KC_DB", "postgres")
            .WithEnvironment("KC_DB_URL_DATABASE", keycloakDb.Resource.DatabaseName)
            .WithEnvironment(context =>
            {
                context.EnvironmentVariables["KC_DB_URL_HOST"] = postgres.Resource.HostName;
                context.EnvironmentVariables["KC_DB_URL_PORT"] = "5432";
                context.EnvironmentVariables["KC_DB_USERNAME"] = postgres.Resource.UserName!.ValueExpression;
                context.EnvironmentVariables["KC_DB_PASSWORD"] = postgres.Resource.Password!.ValueExpression;
            })
            .WaitFor(keycloakDb);

    // API on ACA
    IResourceBuilder<ProjectResource> apiProject = builder.AddProject<Projects.CleanArchitecture_Api>(ApiProjectName)
        .WithReference(appDb)
        .WaitFor(appDb)
        .WithReference(keycloak)
        .WaitFor(keycloak)
        .WithReference(keyVault);

    // Admin app on ACA
    builder.AddNodeApp(AdminAppName, "../../CleanArchitecture.Presentation/admin", "node_modules/next/dist/bin/next")
        .WithHttpEndpoint(targetPort: AdminTargetPort)
        .WithArgs("start")
        .WithPnpm()
        .WithReference(apiProject)
        .WaitFor(apiProject);
}
