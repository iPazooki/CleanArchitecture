﻿using CleanArchitecture.Domain.Entities.Book;
using CleanArchitecture.Domain.Entities.Order;

namespace CleanArchitecture.Application.Entities.OrderItem.Commands.Create;

internal class CreateOrderItemRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<CreateOrderItemCommand>> validators) 
    : BaseRequestHandler<CreateOrderItemCommand>(validators)
{
    protected override async Task<Result> HandleRequest(CreateOrderItemCommand request, CancellationToken cancellationToken)
    {
        Order? order = await applicationUnitOfWork.Orders.FindAsync(keyValues: [request.OrderId], cancellationToken);

        Book? book = await applicationUnitOfWork.Books.FindAsync(keyValues: [request.BookId], cancellationToken);

        if (order is null || book is null)
        {
            return Result.Failure("Order or Book Not Found.");
        }

        order.AddOrderItem(book, request.Quantity, request.Price);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? Result.Success()
            : Result.Failure(string.Format(GeneralErrors.GeneralCreateErrorMessage, nameof(OrderItem)));
    }
}