using CleanArchitecture.Domain.Entities.Order;

namespace CleanArchitecture.Application.Entities.Orders.Queries.Get;

public class GetOrderQueryHandler(IApplicationUnitOfWork applicationUnitOfWork) : IRequestHandler<GetOrderQuery, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        Order? order = await applicationUnitOfWork.Orders
            .Include(o => o.Customer)
            .Include(o=>o.OrderItems)
            .ThenInclude(oi=>oi.Book)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return order is null
            ? Result<OrderResponse>.Failure("Order Not Found.")
            : Result<OrderResponse>.Success(order.ToResponse());
    }
}