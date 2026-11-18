namespace CleanArchitecture.AppHost;

internal static class AppHostConstants
{
    internal const int AdminHostPort = 65499;
    internal const int AdminTargetPort = 3000;
    internal const int KeycloakDevelopmentPort = 8180;
    internal const int KeycloakProductionPort = 8080;
    internal const string KeycloakClientSecretVariable = "KEYCLOAK_CLIENT_SECRET";
    internal const string AdminAppRelativePath = "../../CleanArchitecture.Presentation/admin";
    internal const string ApiProjectPath = "../../CleanArchitecture.Presentation/API/CleanArchitecture.Api.csproj";
    internal const string DbMigratorProjectPath = "../../CleanArchitecture.DbMigrator/CleanArchitecture.DbMigrator.csproj";
    internal const string NextJsEntryPoint = "node_modules/next/dist/bin/next";
}
