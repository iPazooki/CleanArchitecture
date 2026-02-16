namespace CleanArchitecture.Application.Entities.Orders.EventHandlers;

public sealed class OrderAddedEventHandler(ILogger<OrderAddedEventHandler> logger) : INotificationHandler<OrderAddedEvent>
{
    public ValueTask Handle(OrderAddedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", notification.GetType().Name); 
        }

        return ValueTask.CompletedTask;
    }
}
