namespace CleanArchitecture.Application.Entities.Orders.Commands.Update;

public record UpdateOrderCommand(int OrderId, int OrderStatus) : IRequest<Result>;