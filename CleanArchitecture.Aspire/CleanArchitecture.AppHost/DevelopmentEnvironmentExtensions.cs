namespace CleanArchitecture.AppHost;

internal static class DevelopmentEnvironmentExtensions
{
    internal static void ConfigureDevelopmentEnvironment(this IDistributedApplicationBuilder builder)
    {
        IResourceBuilder<PostgresDatabaseResource> postgres = builder.AddPostgres("postgres")
            .WithDataVolume("postgres-data")
            .WithPgAdmin()
            .WithLifetime(ContainerLifetime.Persistent)
            .AddDatabase(ResourceNames.PostgresDatabase, "mrpaneldb");

        IResourceBuilder<ParameterResource> username =
            builder.AddParameter("keycloakAdminUsername", "admin");

        IResourceBuilder<ParameterResource> password =
            builder.AddParameter("keycloakAdminPassword", "admin", secret: true);

        IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", AppHostConstants.KeycloakPort, username, password)
            .WithRealmImport("./Realms")
            .WithDataVolume()
            .WithOtlpExporter()
            .WithLifetime(ContainerLifetime.Persistent);

        IResourceBuilder<ProjectResource> migrator = builder.AddProject<Projects.CleanArchitecture_DbMigrator>(ResourceNames.DbMigrator)
            .WithReference(postgres)
            .WaitFor(postgres);

        IResourceBuilder<ProjectResource> apiProject = builder.AddProject<Projects.CleanArchitecture_Api>(ResourceNames.Api)
            .WithReference(postgres)
            .WaitFor(postgres)
            .WaitFor(migrator)
            .WithReference(keycloak)
            .WaitFor(keycloak);

        builder.AddNodeApp(ResourceNames.Admin, AppHostConstants.AdminAppRelativePath, AppHostConstants.NextJsEntryPoint)
            .WithHttpEndpoint(targetPort: AppHostConstants.AdminTargetPort, port: AppHostConstants.AdminHostPort)
            .WithArgs("dev")
            .WithPnpm()
            .WithReference(apiProject)
            .WaitFor(apiProject);
    }
}
