using CleanArchitecture.Domain.Entities.Order;

namespace CleanArchitecture.Application.Entities.OrderItem.Commands.Delete;

internal class DeleteOrderItemRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<DeleteOrderItemCommand>> validators) 
    : BaseRequestHandler<DeleteOrderItemCommand>(validators)
{
    protected override async Task<Result> HandleRequest(DeleteOrderItemCommand request, CancellationToken cancellationToken)
    {
        Order? order = await applicationUnitOfWork.Orders.FindAsync(keyValues: [request.OrderId], cancellationToken);

        if (order is null)
        {
            return Result<Order>.Failure("Order Not Found.");
        }

        Domain.Entities.Order.OrderItem orderItem = order.OrderItems.Single(x => x.Id == request.OrderItemId);

        order.RemoveOrderItem(orderItem);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? Result.Success()
            : Result.Failure(string.Format(GeneralErrors.GeneralDeleteErrorMessage, nameof(OrderItem)));
    }
}