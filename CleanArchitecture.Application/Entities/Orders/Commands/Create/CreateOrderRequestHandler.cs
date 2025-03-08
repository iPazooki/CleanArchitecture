using CleanArchitecture.Domain.Entities.Order;
using User = CleanArchitecture.Domain.Entities.User.User;

namespace CleanArchitecture.Application.Entities.Orders.Commands.Create;

internal class CreateOrderRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<CreateOrderCommand>> validators ) 
    : BaseRequestHandler<CreateOrderCommand, Guid>(validators)
{
    protected override async Task<Result<Guid>> HandleRequest(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        User? customer = await applicationUnitOfWork.Users.FindAsync(keyValues: [request.CustomerId], cancellationToken);

        if (customer is null)
        {
            return Result<Guid>.Failure("Customer Not Found.");
        }

        Result<Order> order = Order.Create(customer, OrderStatus.Pending);
        
        if (!order.IsSuccess)
        {
            return Result<Guid>.Failure(order.Errors.ToArray());
        }

        applicationUnitOfWork.Orders.Add(order!);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken);
        
        return result.IsSuccess
            ? Result<Guid>.Success(order.Value!.Id)
            : Result<Guid>.Failure(string.Format(GeneralErrors.GeneralCreateErrorMessage, nameof(Order)));
    }
}