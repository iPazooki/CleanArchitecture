using CleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Interceptors;

/// <summary>
/// Dispatches recorded domain events immediately before changes are committed.
/// </summary>
/// <remarks>
/// Dispatching before the commit — rather than after it — puts the handlers inside the same
/// transaction: a handler that throws rolls the write back, and a handler that writes to the
/// context has its changes committed atomically with the change that triggered it. Dispatching
/// from <c>SavedChangesAsync</c> would report a handler failure to the caller as a failed save
/// even though the data had already been made durable.
/// </remarks>
internal sealed class DispatchDomainEventsInterceptor(IDomainEventDispatcher dispatcher) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        ArgumentNullException.ThrowIfNull(eventData);

        await DispatchDomainEventsAsync(eventData.Context, cancellationToken).ConfigureAwait(false);

        return await base.SavingChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask DispatchDomainEventsAsync(DbContext? context, CancellationToken cancellationToken)
    {
        if (context is null)
        {
            return;
        }

        List<AggregateRoot> aggregates = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Count != 0)
            .Select(entry => entry.Entity)
            .ToList();

        List<IDomainEvent> domainEvents = aggregates
            .SelectMany(aggregate => aggregate.DomainEvents)
            .ToList();

        // Cleared before dispatch so a handler that saves again cannot re-publish the same events.
        aggregates.ForEach(aggregate => aggregate.ClearDomainEvents());

        await dispatcher.DispatchAsync(domainEvents, cancellationToken).ConfigureAwait(false);
    }
}
