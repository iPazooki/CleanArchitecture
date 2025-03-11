using System.Text;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Entities.OrderItem.Commands.Create;

internal class CreateOrderItemRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<CreateOrderItemCommand>> validators)
    : BaseRequestHandler<CreateOrderItemCommand>(validators)
{
    private static readonly CompositeFormat _errorMessage = CompositeFormat.Parse(GeneralErrors.GeneralCreateErrorMessage);

    protected override async Task<Result> HandleRequest(CreateOrderItemCommand request,
        CancellationToken cancellationToken)
    {
        Order? order = await applicationUnitOfWork.Orders.FindAsync(keyValues: [request.OrderId], cancellationToken)
            .ConfigureAwait(false);

        Book? book = await applicationUnitOfWork.Books.FindAsync(keyValues: [request.BookId], cancellationToken)
            .ConfigureAwait(false);

        if (order is null || book is null)
        {
            return Result.Failure("Order or Book Not Found.");
        }

        order.AddOrderItem(book, request.Quantity, request.Price);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result.Success()
            : Result.Failure(string.Format(CultureInfo.InvariantCulture, _errorMessage, nameof(OrderItem)));
    }
}
