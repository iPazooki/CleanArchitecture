namespace CleanArchitecture.Application.Entities.OrderItem.Queries;

/// <summary>
/// Response model for an order item query.
/// </summary>
/// <param name="BookName">The name of the book.</param>
/// <param name="Quantity">The quantity of the book.</param>
/// <param name="Price">The price of the book.</param>
public record OrderItemResponse(string BookName, int Quantity, decimal Price);