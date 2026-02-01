using Microsoft.Extensions.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment("Testing"))
{
    IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("testingPostgres");

    IResourceBuilder<PostgresDatabaseResource> postgresdb = postgres.AddDatabase("postgresdb", "testingDb");

    builder.AddProject<Projects.CleanArchitecture_Api>("cleanarchitecture-api")
        .WithReference(postgresdb)
        .WaitFor(postgresdb);
}
else
{
    IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("postgres")
        .WithDataVolume("postgres_data")
        .WithPgAdmin()
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

await builder.Build().RunAsync().ConfigureAwait(false);
