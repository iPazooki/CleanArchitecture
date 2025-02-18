namespace CleanArchitecture.Domain.Events.Order;

public class OrderAddedEvent(Entities.Order.Order order) : INotification
{
    public Entities.Order.Order Order { get; } = order;
}