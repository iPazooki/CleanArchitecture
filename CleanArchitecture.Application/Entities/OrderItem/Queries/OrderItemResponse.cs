namespace CleanArchitecture.Application.Entities.OrderItem.Queries;

public record OrderItemResponse(string BookName, int Quantity, decimal Price);