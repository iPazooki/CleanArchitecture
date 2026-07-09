using System.Reflection;
using DomainValidation;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Persistence.Data;

/// <summary>
/// Represents the application database context.
/// </summary>
/// <param name="options">The options for configuring the database context.</param>
/// <param name="logger">The logger used to record write failures.</param>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger)
    : DbContext(options), IApplicationDbContext
{
    /// <inheritdoc />
    public DbSet<Book> Books => Set<Book>();

    /// <inheritdoc />
    public async Task<Result> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result.Success();
        }
        catch (DbUpdateConcurrencyException exception)
        {
            logger.ConcurrencyConflict(exception);

            return Result.Failure(PersistenceErrors.ConcurrencyConflict);
        }
        catch (DbUpdateException exception)
        {
            // The exception is logged, never returned: its message names tables, columns and
            // constraints, and the Result flows straight into an HTTP response body.
            logger.SaveFailed(exception);

            return Result.Failure(PersistenceErrors.SaveFailed);
        }
    }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types exposed in <see cref="DbSet{TEntity}"/> properties on the derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}

/// <summary>
/// Source-generated log messages for persistence failures.
/// </summary>
internal static partial class PersistenceLog
{
    [LoggerMessage(
        EventId = 3000,
        Level = LogLevel.Error,
        Message = "Saving changes failed.")]
    public static partial void SaveFailed(this ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Warning,
        Message = "Saving changes failed because another transaction modified the same rows.")]
    public static partial void ConcurrencyConflict(this ILogger logger, Exception exception);
}
