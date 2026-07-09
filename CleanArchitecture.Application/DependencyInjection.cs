using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Entities.Books.Commands.Create;
using Microsoft.Extensions.DependencyInjection;

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
        // Validators are internal to keep them out of this assembly's public surface, so
        // FluentValidation needs includeInternalTypes to discover them.
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.GenerateTypesAsInternal = true;
            options.Assemblies = [typeof(CreateBookCommandHandler)];

            // Order matters: the first behavior is the outermost. Logging wraps validation so that
            // validation failures are still logged as failed requests.
            options.PipelineBehaviors = [typeof(LoggingBehaviour<,>), typeof(ValidationBehaviour<,>)];
        });

        return services;
    }
}
