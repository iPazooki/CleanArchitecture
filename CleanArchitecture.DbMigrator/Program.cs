using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Net.Sockets;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(logging =>
        {
            logging.AddConsole();
        });

        services.AddApplicationServices();
        services.AddInfrastructurePersistenceServices(context.Configuration);
    })
    .Build();

using IServiceScope scope = host.Services.CreateScope();

ILogger logger = scope.ServiceProvider
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("DbMigrator");

ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

const int maxAttempts = 10;

TimeSpan retryDelay = TimeSpan.FromSeconds(3);

for (int attempt = 1; attempt <= maxAttempts; attempt++)
{
    try
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            string[] pendingMigrations = [.. await dbContext.Database.GetPendingMigrationsAsync().ConfigureAwait(false)];

            logger.LogInformation(
                "Starting database migration. Attempt {Attempt}/{MaxAttempts}. Pending migrations: {PendingCount}",
                attempt,
                maxAttempts,
                pendingMigrations.Length);
        }

        await dbContext.Database.MigrateAsync().ConfigureAwait(false);

        logger.LogInformation("Database migration completed successfully.");
        return 0;
    }
    catch (Exception ex) when (IsTransient(ex) && attempt < maxAttempts)
    {
        logger.LogWarning(
            ex,
            "Transient failure while applying migrations on attempt {Attempt}/{MaxAttempts}. Retrying in {RetryDelay}.",
            attempt,
            maxAttempts,
            retryDelay);

        await Task.Delay(retryDelay).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed.");
        return 1;
    }
}

logger.LogError("Database migration failed after exhausting retries.");
return 1;

static bool IsTransient(Exception exception)
{
    Exception baseException = exception.GetBaseException();

    return baseException is NpgsqlException
        or TimeoutException
        or IOException
        or SocketException;
}
