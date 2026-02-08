using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using CleanArchitecture.Infrastructure.Persistence.Data;
using CleanArchitecture.Infrastructure.Persistence.Data.UnitOfWork;
using CleanArchitecture.Infrastructure.Persistence.Data.Interceptors;

namespace CleanArchitecture.Infrastructure.Persistence;

/// <summary>
/// Provides extension methods to add infrastructure persistence services to the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the infrastructure persistence services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configuration">The configuration to use for retrieving the connection string.</param>
    /// <returns>The service collection with the added services.</returns>
    public static IServiceCollection AddInfrastructurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Adds the AuditableEntityInterceptor as a scoped service.
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        string? connectionString = configuration.GetConnectionString("postgresdb");

        ArgumentNullException.ThrowIfNull(connectionString);

        // Configures the ApplicationDbContext with the connection string and interceptors.
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseNpgsql(connectionString, builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        // Adds the ApplicationUnitOfWork as a scoped service.
        services.AddScoped<IApplicationUnitOfWork, ApplicationUnitOfWork>();

        // Adds the system time provider as a singleton service.
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
