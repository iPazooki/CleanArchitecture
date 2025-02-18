namespace CleanArchitecture.Application.Entities.OrderItem.Commands.Create;

public record CreateOrderItemCommand(int OrderId, int BookId, int Quantity, decimal Price) : IRequest<Result>;