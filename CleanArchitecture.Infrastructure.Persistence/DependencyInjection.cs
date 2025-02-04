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
        // Retrieves the connection string from the configuration.
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        // Throws an exception if the connection string is null or empty.
        ArgumentException.ThrowIfNullOrEmpty(connectionString, "Connection string 'DefaultConnection' not found.");

        // Adds the AuditableEntityInterceptor as a scoped service.
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        // Configures the ApplicationDbContext with the connection string and interceptors.
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            #if (UseSQLite)
            // Configures the context to use SQLite if the UseSQLite symbol is defined.
            options.UseSqlite(connectionString, builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            #else
            // Configures the context to use SQL Server if the UseSQLite symbol is not defined.
            options.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            #endif
        });

        // Adds the ApplicationUnitOfWork as a scoped service.
        services.AddScoped<IApplicationUnitOfWork, ApplicationUnitOfWork>();

        // Adds the system time provider as a singleton service.
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}