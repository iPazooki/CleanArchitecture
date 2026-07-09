using System.Collections.Frozen;
using CleanArchitecture.Application.Entities.Books.EventHandlers;
using CleanArchitecture.Domain.Events;

namespace CleanArchitecture.Application.Common;

/// <summary>
/// Maps each Domain <see cref="IDomainEvent"/> onto the mediator notification that carries it.
/// </summary>
/// <remarks>
/// This is the seam that keeps <c>Mediator</c> out of the Domain project: domain events know
/// nothing about messaging, and the Application layer decides how each is transported.
/// <para>
/// The map is written out by hand rather than discovered by reflection. Mediator's source
/// generator emits a dispatch arm per concrete notification type, so an open generic wrapper
/// cannot work; and, as with <c>PolicyRegistry</c>, an explicit list stays trimming- and
/// AOT-friendly. To add an event: declare its notification, then add one entry here.
/// </para>
/// </remarks>
internal static class DomainEventNotifications
{
    private static readonly FrozenDictionary<Type, Func<IDomainEvent, INotification>> _factories =
        new Dictionary<Type, Func<IDomainEvent, INotification>>
        {
            [typeof(BookAddedEvent)] = domainEvent => new BookAddedNotification((BookAddedEvent)domainEvent)
        }.ToFrozenDictionary();

    /// <summary>
    /// Wraps <paramref name="domainEvent"/> in its notification.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The event has no registered notification. Failing loudly beats dropping the event silently.
    /// </exception>
    public static INotification Create(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        return _factories.TryGetValue(domainEvent.GetType(), out Func<IDomainEvent, INotification>? factory)
            ? factory(domainEvent)
            : throw new InvalidOperationException(
                $"No notification is registered for domain event '{domainEvent.GetType()}'. " +
                $"Add an entry to {nameof(DomainEventNotifications)}.");
    }
}
