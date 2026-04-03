using CleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Interceptors;

/// <summary>
/// Dispatches domain events after changes have been saved to the database.
/// Only the async path is overridden; the UoW exclusively uses <see cref="DbContext.SaveChangesAsync"/>.
/// </summary>
public class DispatchDomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = new())
    {
        ArgumentNullException.ThrowIfNull(eventData);

        await DispatchDomainEventsAsync(eventData.Context).ConfigureAwait(false);

        return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask DispatchDomainEventsAsync(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        List<AggregateRoot> entities = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        List<INotification> domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ForEach(e => e.ClearDomainEvents());

        foreach (INotification domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent).ConfigureAwait(false);
        }
    }
}