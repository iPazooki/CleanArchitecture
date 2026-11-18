using Aspire.Hosting.Dotnet;

namespace CleanArchitecture.AppHost.Environments;

internal static class DevelopmentEnvironmentExtensions
{
    internal static void ConfigureDevelopmentEnvironment(this IDistributedApplicationBuilder builder)
    {
        IResourceBuilder<PostgresDatabaseResource> postgres = builder.AddPostgres("postgres")
            .WithDataVolume("postgres-data")
            .WithPgAdmin()
            .WithLifetime(ContainerLifetime.Persistent)
            .AddDatabase(ResourceNames.PostgresDatabase, "cleanarchitecturedb");

        IResourceBuilder<ParameterResource> username =
            builder.AddParameter("keycloakAdminUsername", "admin");

        IResourceBuilder<ParameterResource> password =
            builder.AddParameter("keycloakAdminPassword", "admin", secret: true);

        // Persisted so it stays stable across runs: the realm import is IGNORE_EXISTING,
        // so Keycloak keeps whatever secret it saw on first import.
        IResourceBuilder<ParameterResource> adminClientSecret = builder.AddParameter(
            "keycloakClientSecret",
            new GenerateParameterDefault { MinLength = 32, Special = false },
            secret: true,
            persist: true);

        IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", AppHostConstants.KeycloakDevelopmentPort, username, password)
            .WithRealmImport("./Realms")
            .WithEnvironment(AppHostConstants.KeycloakClientSecretVariable, adminClientSecret)
            .WithBindMount("./Keycloak/quarkus.properties", "/opt/keycloak/conf/quarkus.properties", isReadOnly: true)
            .WithDataVolume()
            .WithOtlpExporter()
            .WithLifetime(ContainerLifetime.Persistent);

        IResourceBuilder<DotnetProjectResource> migrator = builder.AddDotnetProject(ResourceNames.DbMigrator, AppHostConstants.DbMigratorProjectPath)
            .WithReference(postgres)
            .WaitFor(postgres);

        IResourceBuilder<DotnetProjectResource> apiProject = builder.AddDotnetProject(ResourceNames.Api, AppHostConstants.ApiProjectPath)
            .WithReference(postgres)
            .WaitFor(postgres)
            .WaitFor(migrator)
            .WithReference(keycloak)
            .WaitFor(keycloak);

        builder.AddNodeApp(ResourceNames.Admin, AppHostConstants.AdminAppRelativePath, AppHostConstants.NextJsEntryPoint)
            .WithHttpEndpoint(targetPort: AppHostConstants.AdminTargetPort, port: AppHostConstants.AdminHostPort)
            .WithArgs("dev")
            .WithPnpm()
            .WithEnvironment(AppHostConstants.KeycloakClientSecretVariable, adminClientSecret)
            .WithReference(apiProject)
            .WaitFor(apiProject);
    }
}
