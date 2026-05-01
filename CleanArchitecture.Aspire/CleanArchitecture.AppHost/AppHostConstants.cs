namespace CleanArchitecture.AppHost;

internal static class AppHostConstants
{
    internal const string TestingEnvironment = "Testing";

    internal const int AdminHostPort = 65499;
    internal const int AdminTargetPort = 3000;
    internal const int KeycloakPort = 8080;

    internal const string AdminAppRelativePath = "../../CleanArchitecture.Presentation/admin";
    internal const string NextJsEntryPoint = "node_modules/next/dist/bin/next";
}
