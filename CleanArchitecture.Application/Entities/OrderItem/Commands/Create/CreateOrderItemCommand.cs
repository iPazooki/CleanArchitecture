namespace CleanArchitecture.Application.Entities.OrderItem.Commands.Create;

/// <summary>
/// Command for creating an order item.
/// </summary>
/// <param name="OrderId">The ID of the order.</param>
/// <param name="BookId">The ID of the book.</param>
/// <param name="Quantity">The quantity of the book.</param>
/// <param name="Price">The price of the book.</param>
/// <returns>A result indicating the success or failure of the command.</returns>
public record CreateOrderItemCommand(int OrderId, int BookId, int Quantity, decimal Price) : IRequest<Result>;