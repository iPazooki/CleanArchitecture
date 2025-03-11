using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Entities.Orders.Queries.Get;

internal class GetOrderQueryHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<GetOrderQuery>> validators)
    : BaseRequestHandler<GetOrderQuery, OrderResponse>(validators)
{
    protected override async Task<Result<OrderResponse>> HandleRequest(GetOrderQuery request, CancellationToken cancellationToken)
    {
        Order? order = await applicationUnitOfWork.Orders
            .Include(o => o.Customer)
            .Include(o=>o.OrderItems)
            .ThenInclude(oi=>oi.Book)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken).ConfigureAwait(false);

        return order is null
            ? Result<OrderResponse>.Failure("Order Not Found.")
            : Result<OrderResponse>.Success(order.ToResponse());
    }
}
