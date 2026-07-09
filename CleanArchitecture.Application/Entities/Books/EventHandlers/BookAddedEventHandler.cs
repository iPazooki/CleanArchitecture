using CleanArchitecture.Domain.Events;

namespace CleanArchitecture.Application.Entities.Books.EventHandlers;

/// <summary>
/// Carries a <see cref="BookAddedEvent"/> across the mediator.
/// </summary>
/// <param name="DomainEvent">The domain event that occurred.</param>
internal sealed record BookAddedNotification(BookAddedEvent DomainEvent) : INotification;

/// <summary>
/// Reacts to a book being added to the catalog.
/// </summary>
internal sealed class BookAddedEventHandler(ILogger<BookAddedEventHandler> logger)
    : INotificationHandler<BookAddedNotification>
{
    public ValueTask Handle(BookAddedNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        logger.BookAdded(notification.DomainEvent.BookId);

        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// Source-generated log messages for the book event handlers.
/// </summary>
internal static partial class BookEventLog
{
    [LoggerMessage(
        EventId = 2100,
        Level = LogLevel.Information,
        Message = "Domain event: book {BookId} was added to the catalog.")]
    public static partial void BookAdded(this ILogger logger, Guid bookId);
}
