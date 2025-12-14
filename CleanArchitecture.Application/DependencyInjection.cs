using System.Reflection;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Entities.Books.Commands.Create;
using CleanArchitecture.Domain.Entities;
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
        // Adds validators from the executing assembly to the service collection.
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.GenerateTypesAsInternal = true;
            options.Assemblies = [typeof(CreateBookCommandHandler), typeof(Book)];
            options.PipelineBehaviors = [typeof(LoggingBehaviour<,>)];
        });

        return services;
    }
}
