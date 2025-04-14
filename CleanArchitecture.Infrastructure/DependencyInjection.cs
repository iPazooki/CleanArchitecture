using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.Infrastructure.Security;

namespace CleanArchitecture.Infrastructure;

/// <summary>
/// Provides extension methods for setting up infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection with the added services.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Adds the EmailService to the service collection with a scoped lifetime.
        services.AddScoped<IEmailService, EmailServiceProvider.EmailService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();

        return services;
    }
}