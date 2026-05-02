namespace CleanArchitecture.AppHost.Environments;

internal static class TestingEnvironmentExtensions
{
    internal static void ConfigureTestingEnvironment(this IDistributedApplicationBuilder builder)
    {
        IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres("testingPostgres");

        IResourceBuilder<PostgresDatabaseResource> postgresdb =
            postgres.AddDatabase(ResourceNames.PostgresDatabase, "testingDb");

        IResourceBuilder<ProjectResource> migrator = builder.AddProject<Projects.CleanArchitecture_DbMigrator>(ResourceNames.DbMigrator)
            .WithReference(postgresdb)
            .WaitFor(postgresdb)
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", AppHostConstants.TestingEnvironment);

        IResourceBuilder<ProjectResource> project = builder.AddProject<Projects.CleanArchitecture_Api>(ResourceNames.Api)
            .WithReference(postgresdb)
            .WaitFor(postgresdb)
            .WaitFor(migrator);

        project.WithEnvironment("ASPNETCORE_ENVIRONMENT", AppHostConstants.TestingEnvironment);
    }
}
