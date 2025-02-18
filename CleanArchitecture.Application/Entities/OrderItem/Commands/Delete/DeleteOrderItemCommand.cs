namespace CleanArchitecture.Application.Entities.OrderItem.Commands.Delete;

public record DeleteOrderItemCommand(int OrderId, int OrderItemId) : IRequest<Result>;