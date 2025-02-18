namespace CleanArchitecture.Domain.Events.Order;

public class OrderStatusChangedEvent(Entities.Order.Order order) : INotification
{
    public Entities.Order.Order Order { get; } = order;
}