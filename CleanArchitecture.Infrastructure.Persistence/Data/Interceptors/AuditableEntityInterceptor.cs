using CleanArchitecture.Domain.Common;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Interceptors;

/// <summary>
/// Stamps auditable entities with creation and update timestamps before changes are saved.
/// </summary>
/// <param name="timeProvider">The provider for getting the current time.</param>
internal sealed class AuditableEntityInterceptor(TimeProvider timeProvider, ICurrentUser currentUser) : SaveChangesInterceptor
{
    /// <summary>
    /// Called before changes are saved to the database synchronously.
    /// </summary>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Called before changes are saved to the database asynchronously.
    /// </summary>
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

        // Read the clock once: every entity committed in the same transaction should carry the
        // same timestamp, not one each depending on how long the loop took.
        DateTimeOffset utcNow = timeProvider.GetUtcNow();

        foreach (EntityEntry<EntityAuditable> entry in context.ChangeTracker.Entries<EntityAuditable>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified) && !HasChangedOwnedEntities(entry))
            {
                continue;
            }

            // Written through the change tracker because the domain exposes these as private-set:
            // auditing is the persistence layer's job, not something a caller can forge.
            if (entry.State == EntityState.Added)
            {
                entry.Property(auditable => auditable.CreatedDate).CurrentValue = utcNow;
            }

            entry.Property(auditable => auditable.UpdatedDate).CurrentValue = utcNow;
            entry.Property(auditable => auditable.UpdatedBy).CurrentValue = currentUser.UserName;
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
