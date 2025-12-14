namespace CleanArchitecture.Application.Entities.Orders.EventHandlers;

public sealed class OrderStatusChangedEventHandler(ILogger<OrderStatusChangedEventHandler> logger) : INotificationHandler<OrderStatusChangedEvent>
{
    public ValueTask Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        logger.LogInformation("CleanArchitecture Domain Event: {@DomainEvent}", notification.GetType().Name);

        return ValueTask.CompletedTask;
    }
}
