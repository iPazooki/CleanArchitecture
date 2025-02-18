using CleanArchitecture.Domain.Events.Order;

namespace CleanArchitecture.Domain.Entities.Order;

public partial class Order
{
    private Order() { }

    private readonly List<OrderItem> _orderItems = [];
    private OrderStatus _orderStatus;

    public static Result<Order> Create(Person.Person person, OrderStatus orderStatus)
    {
        if (orderStatus!= OrderStatus.Pending)
        {
            return Result<Order>.Failure(OrderErrors.OrderStatusInvalid);
        }

        Order order = new() { Customer = person, OrderStatus = orderStatus };
        
        order.AddDomainEvent(new OrderAddedEvent(order));

        return order;
    }

    public Result AddOrderItem(Book.Book book, int quantity, decimal unitPrice)
    {
        HashSet<Error> errors = [];
        
        if (quantity <= 0)
        {
            errors.Add(OrderErrors.OrderItemQuantityInvalid);
        }

        if (unitPrice <= 0)
        {
            errors.Add(OrderErrors.OrderItemUnitPriceInvalid);
        }
        
        if (errors.Count != 0)
        {
            return Result.Failure(errors.ToArray());
        }

        OrderItem orderItem = new() { Book = book, Quantity = quantity, UnitPrice = unitPrice };

        _orderItems.Add(orderItem);
        
        return Result.Success();
    }

    public void RemoveOrderItem(OrderItem orderItem) => _orderItems.Remove(orderItem);
}