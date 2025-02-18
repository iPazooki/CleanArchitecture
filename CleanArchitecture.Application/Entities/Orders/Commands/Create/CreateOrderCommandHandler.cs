using CleanArchitecture.Domain.Entities.Order;
using CleanArchitecture.Domain.Entities.Person;

namespace CleanArchitecture.Application.Entities.Orders.Commands.Create;

public class CreateOrderCommandHandler(IApplicationUnitOfWork applicationUnitOfWork) : IRequestHandler<CreateOrderCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        Person? customer = await applicationUnitOfWork.Persons.FindAsync(keyValues: [request.CustomerId], cancellationToken);

        if (customer is null)
        {
            return Result<int>.Failure("Customer Not Found.");
        }

        Result<Order> order = Order.Create(customer, OrderStatus.Pending);
        
        if (!order.IsSuccess)
        {
            return Result<int>.Failure(order.Errors.ToArray());
        }

        applicationUnitOfWork.Orders.Add(order!);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken);
        
        return result.IsSuccess
            ? Result<int>.Success(order.Value!.Id)
            : Result<int>.Failure(string.Format(GeneralErrors.GeneralCreateErrorMessage, nameof(Order)));
    }
}