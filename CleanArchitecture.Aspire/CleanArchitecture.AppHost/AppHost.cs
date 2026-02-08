using Microsoft.Extensions.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment("Testing"))
{
    await ConfigureTestingEnvironment(builder).ConfigureAwait(false);
}
else if(builder.Environment.IsDevelopment())
{
    await ConfigureDevelopmentEnvironment(builder).ConfigureAwait(false);
}
else
{
    await ConfigureProductionEnvironment(builder).ConfigureAwait(false);
}

await builder.Build().RunAsync().ConfigureAwait(false);


static async Task ConfigureTestingEnvironment(IDistributedApplicationBuilder builder)
{
    IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("testingPostgres");

    IResourceBuilder<PostgresDatabaseResource> postgresdb = postgres.AddDatabase("postgresdb", "testingDb");

    builder.AddProject<Projects.CleanArchitecture_Api>("cleanarchitecture-api")
        .WithReference(postgresdb)
        .WaitFor(postgresdb);
}

static async Task ConfigureDevelopmentEnvironment(IDistributedApplicationBuilder builder)
{
    IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("postgres")
        .WithDataVolume("postgres_data")
        .WithPgAdmin()
        .WithLifetime(ContainerLifetime.Persistent);

    IResourceBuilder<PostgresDatabaseResource> postgresdb = postgres.AddDatabase("postgresdb", "cleandb");

    IResourceBuilder<ParameterResource> username = builder.AddParameter("keycloakAdminUsername", () => "admin");
    IResourceBuilder<ParameterResource> password = builder.AddParameter("keycloakAdminPassword", () => "admin", secret: true);

    IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", 8080, username, password)
        .WithRealmImport("./Realms")
        .WithDataVolume()
        .WithOtlpExporter()
        .WithLifetime(ContainerLifetime.Persistent);

    builder.AddProject<Projects.CleanArchitecture_Api>("cleanarchitecture-api")
        .WithReference(postgresdb)
        .WaitFor(postgresdb)
        .WithReference(keycloak)
        .WaitFor(keycloak);
}


static async Task ConfigureProductionEnvironment(IDistributedApplicationBuilder builder)
{
    IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("postgres")
        .WithDataVolume("postgres_data")
        .WithLifetime(ContainerLifetime.Persistent);

    IResourceBuilder<PostgresDatabaseResource> postgresdb = postgres.AddDatabase("postgresdb", "cleandb");

    IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", 8080)
        .WithDataVolume()
        .WithOtlpExporter()
        .WithLifetime(ContainerLifetime.Persistent);

    builder.AddProject<Projects.CleanArchitecture_Api>("cleanarchitecture-api")
        .WithReference(postgresdb)
        .WaitFor(postgresdb)
        .WithReference(keycloak)
        .WaitFor(keycloak);
}
