using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment("Testing"))
{
    var postgres = builder.AddPostgres("testingPostgres");

    var postgresdb = postgres.AddDatabase("postgresdb", "testingDb");

    builder.AddProject<Projects.CleanArchitecture_Api>("cleanarchitecture-api")
        .WithReference(postgresdb)
        .WaitFor(postgresdb);
}
else
{
    var postgres = builder.AddPostgres("postgres")
        .WithDataVolume("postgres_data")
        .WithPgAdmin()
        .WithLifetime(ContainerLifetime.Persistent);

    var postgresdb = postgres.AddDatabase("postgresdb", "cleandb");

    builder.AddProject<Projects.CleanArchitecture_Api>("cleanarchitecture-api")
        .WithReference(postgresdb)
        .WaitFor(postgresdb);
}

await builder.Build().RunAsync().ConfigureAwait(false);
