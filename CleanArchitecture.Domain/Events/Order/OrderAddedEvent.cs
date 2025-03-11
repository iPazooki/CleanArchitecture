namespace CleanArchitecture.Domain.Events.Order;

public class OrderAddedEvent(Entities.Order order) : INotification
{
    public Entities.Order Order { get; } = order;
}
