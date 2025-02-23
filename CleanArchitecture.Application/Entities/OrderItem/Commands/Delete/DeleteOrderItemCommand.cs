namespace CleanArchitecture.Application.Entities.OrderItem.Commands.Delete;

/// <summary>
/// Command for deleting an order item.
/// </summary>
/// <param name="OrderId">The ID of the order.</param>
/// <param name="OrderItemId">The ID of the order item.</param>
/// <returns>A result indicating the success or failure of the command.</returns>
public record DeleteOrderItemCommand(Guid OrderId, Guid OrderItemId) : IRequest<Result>;