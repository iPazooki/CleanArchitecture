using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.Security;

/// <summary>
/// Registers the application's authorization policies.
/// </summary>
internal static class SecurityServiceCollectionExtensions
{
    /// <summary>
    /// Adds every policy in <see cref="PolicyRegistry"/> to the authorization system.
    /// </summary>
    /// <param name="services">The service collection to add the policies to.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        AuthorizationBuilder builder = services.AddAuthorizationBuilder();

        foreach (IAuthorizationPolicyDefinition definition in PolicyRegistry.All)
        {
            builder.AddPolicy(definition.Name, definition.Configure);
        }

        return services;
    }
}
