namespace CleanArchitecture.Api.Configuration;

internal static class EnvironmentExtensions
{
    public static bool IsTesting(this IWebHostEnvironment env)
    {
        ArgumentNullException.ThrowIfNull(env);

        return env.IsEnvironment("Testing");
    }
}
