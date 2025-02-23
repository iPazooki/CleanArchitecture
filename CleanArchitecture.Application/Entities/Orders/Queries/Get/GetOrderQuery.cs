namespace CleanArchitecture.Application.Entities.Orders.Queries.Get;

/// <summary>
/// Query for getting an order by ID.
/// </summary>
/// <param name="Id">The ID of the order.</param>
/// <returns>A result containing the order response.</returns>
public record GetOrderQuery(Guid Id) : IRequest<Result<OrderResponse>>;