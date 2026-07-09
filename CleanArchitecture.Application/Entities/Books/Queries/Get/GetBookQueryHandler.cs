using CleanArchitecture.Domain.Validations.Book;

namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

internal sealed class GetBookQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
    : IRequestHandler<GetBookQuery, Result<BookResponse>>
{
    public async ValueTask<Result<BookResponse>> Handle(GetBookQuery request, CancellationToken cancellationToken)
    {
        BookResponse? book = await applicationUnitOfWork.Books
            .AsNoTracking()
            .Where(b => b.Id == request.Id)
            .Select(BookMappings.Projection)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return book is null
            ? Result<BookResponse>.Failure(BookErrors.BookNotFound)
            : Result<BookResponse>.Success(book);
    }
}
