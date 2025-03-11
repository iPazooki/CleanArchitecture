using System.Text;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Entities.Orders.Commands.Create;

internal class CreateOrderRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<CreateOrderCommand>> validators)
    : BaseRequestHandler<CreateOrderCommand, Guid>(validators)
{
    private static readonly CompositeFormat _errorMessage = CompositeFormat.Parse(GeneralErrors.GeneralCreateErrorMessage);

    protected override async Task<Result<Guid>> HandleRequest(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        User? customer = await applicationUnitOfWork.Users.FindAsync(keyValues: [request.CustomerId], cancellationToken).ConfigureAwait(false);

        if (customer is null)
        {
            return Result<Guid>.Failure("Customer Not Found.");
        }

        Result<Order> order = Order.Create(customer, OrderStatus.Pending);

        if (!order.IsSuccess)
        {
            return Result<Guid>.Failure(order.Errors.ToArray());
        }

        await applicationUnitOfWork.Orders.AddAsync(order!, cancellationToken).ConfigureAwait(false);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result<Guid>.Success(order.Value!.Id)
            : Result<Guid>.Failure(string.Format(CultureInfo.InvariantCulture, _errorMessage, nameof(Order)));
    }
}
