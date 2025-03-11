using CleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Interceptors;

/// <summary>
/// Interceptor for handling auditable entities during save changes operations.
/// </summary>
/// <param name="timeProvider">The provider for getting the current time.</param>
public class AuditableEntityInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    /// <summary>
    /// Called after changes have been saved to the database.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="result">The result of the save operation.</param>
    /// <returns>The result of the save operation.</returns>
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        UpdateEntities(eventData.Context);

        return base.SavedChanges(eventData, result);
    }

    /// <summary>
    /// Called before changes are saved to the database asynchronously.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="result">The result of the save operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = new())
    {
        ArgumentNullException.ThrowIfNull(eventData);

        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Updates the auditable entities in the context.
    /// </summary>
    /// <param name="context">The database context.</param>
    private void UpdateEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        foreach (EntityEntry<EntityAuditable> entry in context.ChangeTracker.Entries<EntityAuditable>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified) && !HasChangedOwnedEntities(entry))
            {
                continue;
            }

            DateTimeOffset utcNow = timeProvider.GetUtcNow();

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = utcNow;
            }

            entry.Entity.UpdatedDate = utcNow;
        }
    }

    private static bool HasChangedOwnedEntities(EntityEntry entry)
    {
        return entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
    }
}
