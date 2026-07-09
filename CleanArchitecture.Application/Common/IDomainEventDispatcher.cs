namespace CleanArchitecture.Application.Common;

/// <summary>
/// Publishes the domain events an aggregate recorded during a unit of work.
/// </summary>
/// <remarks>
/// Exposed as an abstraction so the persistence layer can dispatch events without referencing
/// the mediator, and so the transport can be swapped (for an outbox, say) without touching
/// either the Domain or the persistence interceptor.
/// </remarks>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Publishes each event to its registered handlers, in order.
    /// </summary>
    /// <param name="domainEvents">The events to publish.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
