using CleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Interceptors;

public class DispatchDomainEventsInterceptor(IMediator mediator) : SaveChangesInterceptor
{
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        DispatchDomainEvents(eventData.Context);

        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = new())
    {
        ArgumentNullException.ThrowIfNull(eventData);

        await DispatchDomainEventsAsync(eventData.Context).ConfigureAwait(false);

        return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask DispatchDomainEventsAsync(DbContext? context)
    {
        if (context == null) return;

        IEnumerable<AggregateRoot> entities = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        List<INotification> domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (INotification domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent).ConfigureAwait(false);
        }
    }

    private void DispatchDomainEvents(DbContext? context)
    {
        if (context == null) return;

        IEnumerable<AggregateRoot> entities = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        List<INotification> domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (INotification domainEvent in domainEvents)
        {
            // Synchronously wait for the ValueTask to complete
            mediator.Publish(domainEvent).AsTask().GetAwaiter().GetResult();
        }
    }
}
