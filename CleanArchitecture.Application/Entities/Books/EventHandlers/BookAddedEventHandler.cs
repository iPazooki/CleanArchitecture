using CleanArchitecture.Domain.Events.Book;

namespace CleanArchitecture.Application.Entities.Books.EventHandlers;

public sealed class BookAddedEventHandler(ILogger<BookAddedEventHandler> logger) : INotificationHandler<BookAddedEvent>
{
    public ValueTask Handle(BookAddedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", notification.GetType().Name);
        }

        return ValueTask.CompletedTask;
    }
}
