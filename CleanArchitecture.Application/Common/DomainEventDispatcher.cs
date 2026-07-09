namespace CleanArchitecture.Application.Common;

/// <summary>
/// Publishes domain events through the mediator, each wrapped in its notification.
/// </summary>
/// <remarks>
/// Publishing through the <see cref="INotification"/> static type is safe: Mediator's generated
/// <c>Publish</c> switches on the notification's runtime type, so each event still reaches the
/// handler registered for its concrete notification.
/// </remarks>
internal sealed class DomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await publisher.Publish(DomainEventNotifications.Create(domainEvent), cancellationToken).ConfigureAwait(false);
        }
    }
}
