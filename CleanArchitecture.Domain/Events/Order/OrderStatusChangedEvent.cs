namespace CleanArchitecture.Domain.Events.Order;

public class OrderStatusChangedEvent(Entities.Order order) : INotification
{
    public Entities.Order Order { get; } = order;
}
