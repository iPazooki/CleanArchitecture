namespace CleanArchitecture.Application.Entities.Orders.Commands.Create;

public record CreateOrderCommand(int CustomerId) : IRequest<Result<int>>;