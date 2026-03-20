using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

const string TestingEnvironment = "Testing";
const string AdminAppName = "admin";
const string ApiProjectName = "cleanarchitecture-api";
const string PostgresDatabaseResourceName = "postgresdb";
const string KeyVaultUriConfigurationKey = "KeyVaultUri";
const string KeycloakAdminUsernameSecretNameConfigurationKey = "KeyVaultSecrets:KeycloakAdminUsernameSecretName";
const string KeycloakAdminPasswordSecretNameConfigurationKey = "KeyVaultSecrets:KeycloakAdminPasswordSecretName";

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

    (string keycloakAdminUsername, string keycloakAdminPassword) = ResolveKeycloakAdminCredentials(builder.Configuration);

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

    (string keycloakAdminUsername, string keycloakAdminPassword) = ResolveKeycloakAdminCredentials(builder.Configuration);

    IResourceBuilder<ParameterResource> username =
        builder.AddParameter("keycloakAdminUsername", () => keycloakAdminUsername);

    IResourceBuilder<ParameterResource> password =
        builder.AddParameter("keycloakAdminPassword", () => keycloakAdminPassword, secret: true);

    IResourceBuilder<KeycloakResource> keycloak = builder.AddKeycloak("keycloak", KeycloakPort, username, password)
        .WithExternalHttpEndpoints()
        .WithDataVolume()
        .WithOtlpExporter()
        .WithLifetime(ContainerLifetime.Persistent);

    builder.AddProject<Projects.CleanArchitecture_Api>(ApiProjectName)
        .WithExternalHttpEndpoints()
        .WithReference(postgresdb)
        .WaitFor(postgresdb)
        .WithReference(keycloak)
        .WaitFor(keycloak);
}

static (string Username, string Password) ResolveKeycloakAdminCredentials(IConfiguration configuration)
{
    string? username = Environment.GetEnvironmentVariable("KEYCLOAK_ADMIN_USERNAME");
    string? password = Environment.GetEnvironmentVariable("KEYCLOAK_ADMIN_PASSWORD");

    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
    {
        return (username, password);
    }

    string? keyVaultUri = configuration[KeyVaultUriConfigurationKey];

    if (string.IsNullOrWhiteSpace(keyVaultUri) ||
        !Uri.TryCreate(keyVaultUri, UriKind.Absolute, out Uri? keyVaultEndpoint))
    {
        throw new InvalidOperationException(
            "Keycloak admin credentials are not set in environment variables and Key Vault is not configured. " +
            $"Set '{KeyVaultUriConfigurationKey}' to an absolute Azure Key Vault URI and configure the secret names, or provide both KEYCLOAK_ADMIN_USERNAME and KEYCLOAK_ADMIN_PASSWORD.");
    }

    SecretClient secretClient = new(keyVaultEndpoint, new DefaultAzureCredential());

    username ??= GetRequiredSecretValue(
        secretClient,
        configuration[KeycloakAdminUsernameSecretNameConfigurationKey] ?? "keycloak-admin-username");

    password ??= GetRequiredSecretValue(
        secretClient,
        configuration[KeycloakAdminPasswordSecretNameConfigurationKey] ?? "keycloak-admin-password");

    return (username, password);
}

static string GetRequiredSecretValue(SecretClient secretClient, string secretName)
{
    try
    {
        KeyVaultSecret secret = secretClient.GetSecret(secretName).Value;

        return string.IsNullOrWhiteSpace(secret.Value)
            ? throw new InvalidOperationException($"The Azure Key Vault secret '{secretName}' is empty.")
            : secret.Value;
    }
    catch (CredentialUnavailableException ex)
    {
        throw new InvalidOperationException(
            "Azure credentials are unavailable. Authenticate locally with Visual Studio or Azure CLI, or run with a managed identity.",
            ex);
    }
    catch (RequestFailedException ex)
    {
        throw new InvalidOperationException(
            $"Failed to retrieve Azure Key Vault secret '{secretName}'. Ensure the vault URI and secret name are correct and access is granted.",
            ex);
    }
}
