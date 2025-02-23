namespace CleanArchitecture.Application.Entities.Orders.Commands.Create;

/// <summary>
/// Command for creating an order.
/// </summary>
/// <param name="CustomerId">The ID of the customer.</param>
/// <returns>A result containing a GUID representing the order ID.</returns>
public record CreateOrderCommand(int CustomerId) : IRequest<Result<Guid>>;