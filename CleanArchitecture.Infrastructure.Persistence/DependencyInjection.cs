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
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        string connectionString = configuration.GetConnectionString("postgresDatabaseResource")
            ?? throw new InvalidOperationException(
                "Connection string 'postgresDatabaseResource' not found. " +
                "Use ApplicationDbContextFactory for design-time operations.");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseNpgsql(connectionString, builder =>
                builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        services.AddScoped<IApplicationUnitOfWork, ApplicationUnitOfWork>();

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}