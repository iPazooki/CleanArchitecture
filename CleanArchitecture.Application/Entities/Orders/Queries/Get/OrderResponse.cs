using CleanArchitecture.Application.Entities.OrderItem.Queries;

namespace CleanArchitecture.Application.Entities.Orders.Queries.Get;

public record OrderResponse(int Id, string CustomerName, string OrderStatus, IEnumerable<OrderItemResponse> OrderItems);