using System.Text;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Entities.OrderItem.Commands.Delete;

internal class DeleteOrderItemRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<DeleteOrderItemCommand>> validators)
    : BaseRequestHandler<DeleteOrderItemCommand>(validators)
{
    private static readonly CompositeFormat _errorMessage =
        CompositeFormat.Parse(GeneralErrors.GeneralDeleteErrorMessage);

    protected override async Task<Result> HandleRequest(DeleteOrderItemCommand request, CancellationToken cancellationToken)
    {
        Order? order = await applicationUnitOfWork.Orders.FindAsync(keyValues: [request.OrderId], cancellationToken)
            .ConfigureAwait(false);

        if (order is null)
        {
            return Result<Order>.Failure("Order Not Found.");
        }

        Domain.Entities.OrderItem orderItem = order.OrderItems.Single(x => x.Id == request.OrderItemId);

        order.RemoveOrderItem(orderItem);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result.Success()
            : Result.Failure(string.Format(CultureInfo.InvariantCulture, _errorMessage, nameof(OrderItem)));
    }
}
