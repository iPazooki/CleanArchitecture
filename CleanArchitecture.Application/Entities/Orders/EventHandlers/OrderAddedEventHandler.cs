namespace CleanArchitecture.Application.Entities.Orders.EventHandlers;

public sealed class OrderAddedEventHandler(ILogger<OrderAddedEventHandler> logger) : INotificationHandler<OrderAddedEvent>
{
    public Task Handle(OrderAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}