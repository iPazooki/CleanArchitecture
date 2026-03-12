using Microsoft.Extensions.Hosting;

const string TestingEnvironment = "Testing";
const string AdminAppName = "admin";
const string ApiProjectName = "cleanarchitecture-api";
const string PostgresDatabaseResourceName = "postgresdb";

const int AdminHostPort = 65499;   // Pinned for stable local dev URL
const int AdminTargetPort = 3000;  // Next.js internal port
const int KeycloakPort = 8080;     // Pinned for stable auth callback/local access

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment(TestingEnvironment))
{
    ConfigureTestingEnvironment(builder);
}
else if (builder.Environment.IsDevelopment())
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
        .AddDatabase(PostgresDatabaseResourceName, "cleandb");

    string keycloakAdminUsername = Environment.GetEnvironmentVariable("KEYCLOAK_ADMIN_USERNAME") ?? "admin";
    string keycloakAdminPassword = Environment.GetEnvironmentVariable("KEYCLOAK_ADMIN_PASSWORD") ?? "admin";

    IResourceBuilder<ParameterResource> username =
        builder.AddParameter("keycloakAdminUsername", () => keycloakAdminUsername);

    IResourceBuilder<ParameterResource> password =
        builder.AddParameter("keycloakAdminPassword", () => keycloakAdminPassword, secret: true);

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
    IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("postgres")
        .WithDataVolume("postgres_data")
        .WithLifetime(ContainerLifetime.Persistent);

    IResourceBuilder<PostgresDatabaseResource> postgresdb =
        postgres.AddDatabase(PostgresDatabaseResourceName, "cleandb");

    IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", KeycloakPort)
        .WithDataVolume()
        .WithOtlpExporter()
        .WithLifetime(ContainerLifetime.Persistent);

    builder.AddProject<Projects.CleanArchitecture_Api>(ApiProjectName)
        .WithReference(postgresdb)
        .WaitFor(postgresdb)
        .WithReference(keycloak)
        .WaitFor(keycloak);
}
