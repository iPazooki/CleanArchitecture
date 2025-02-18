namespace CleanArchitecture.Application.Entities.Orders.Queries.Get;

public record GetOrderQuery(int Id) : IRequest<Result<OrderResponse>>;