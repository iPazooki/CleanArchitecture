using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Errors;

namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

internal class GetBookQueryHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<GetBookQuery>> validators)
    : BaseRequestHandler<GetBookQuery, BookResponse>(validators)
{
    protected override async Task<Result<BookResponse>> HandleRequest(GetBookQuery request, CancellationToken cancellationToken)
    {
        Book? book = await applicationUnitOfWork.Books.FindAsync(keyValues: [request.Id], cancellationToken).ConfigureAwait(false);

        return book is null
            ? Result<BookResponse>.Failure(BookErrors.BookNotFound)
            : Result<BookResponse>.Success(book.ToResponse());
    }
}
