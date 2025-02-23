namespace CleanArchitecture.Application.Entities.Orders.Commands.Update;

/// <summary>
/// Command for updating an order.
/// </summary>
/// <param name="OrderId">The ID of the order.</param>
/// <param name="OrderStatus">The status of the order.</param>
/// <returns>A result indicating the success or failure of the command.</returns>
public record UpdateOrderCommand(int OrderId, int OrderStatus) : IRequest<Result>;