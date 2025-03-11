using System.Text;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Entities.Orders.Commands.Update;

internal class UpdateOrderRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<UpdateOrderCommand>> validators)
    : BaseRequestHandler<UpdateOrderCommand>(validators)
{
    private static readonly CompositeFormat _errorMessage = CompositeFormat.Parse(GeneralErrors.GeneralUpdateErrorMessage);

    protected override async Task<Result> HandleRequest(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        Order? order = await applicationUnitOfWork.Orders.FindAsync(keyValues: [request.OrderId], cancellationToken).ConfigureAwait(false);

        if (order is null)
        {
            return Result<Order>.Failure("Order Not Found.");
        }

        if (!Enum.IsDefined(typeof(OrderStatus), request.OrderStatus))
        {
            return Result<Order>.Failure("Invalid Order Status.");
        }

        order.OrderStatus = (OrderStatus)request.OrderStatus;

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result.Success()
            : Result.Failure(string.Format(CultureInfo.InvariantCulture, _errorMessage, nameof(Order)));
    }
}
