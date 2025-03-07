using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.Application.Common.Behaviours;

namespace CleanArchitecture.Application;

/// <summary>
/// Provides extension methods for setting up application services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection with the added services.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Adds validators from the executing assembly to the service collection.
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Adds MediatR services and registers behaviors from the executing assembly.
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        });

        return services;
    }
}