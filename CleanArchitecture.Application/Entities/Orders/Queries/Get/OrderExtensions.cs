using CleanArchitecture.Domain.Entities.Order;
using CleanArchitecture.Application.Entities.OrderItem.Queries;

namespace CleanArchitecture.Application.Entities.Orders.Queries.Get;

public static class OrderExtensions
{
    public static OrderResponse ToResponse(this Order order) =>
        new(Id: order.Id,
            CustomerName: $"{order.Customer.FirstName} {order.Customer.LastName}",
            OrderStatus: order.OrderStatus.ToString(),
            OrderItems: order.OrderItems.Select(o =>
                new OrderItemResponse(o.Book.Title, o.Quantity, o.UnitPrice)).ToList());
}