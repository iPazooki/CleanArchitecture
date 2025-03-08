using CleanArchitecture.Domain.Entities.Order;

namespace CleanArchitecture.Application.Entities.Orders.Commands.Update;

internal class UpdateOrderRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<UpdateOrderCommand>> validators)
    : BaseRequestHandler<UpdateOrderCommand>(validators)
{
    protected override async Task<Result> HandleRequest(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        Order? order = await applicationUnitOfWork.Orders.FindAsync(keyValues: [request.OrderId], cancellationToken);

        if (order is null)
        {
            return Result<Order>.Failure("Order Not Found.");
        }

        if (!Enum.IsDefined(typeof(OrderStatus), request.OrderStatus))
        {
            return Result<Order>.Failure("Invalid Order Status.");
        }

        order.OrderStatus = (OrderStatus)request.OrderStatus;

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? Result.Success()
            : Result.Failure(string.Format(GeneralErrors.GeneralUpdateErrorMessage, nameof(Order)));
    }
}