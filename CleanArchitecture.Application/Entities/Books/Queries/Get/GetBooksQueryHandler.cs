namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

internal class GetBooksQueryHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<GetBooksQuery>> validators)
    : BaseRequestHandler<GetBooksQuery, IEnumerable<BookResponse>>(validators)
{
    protected override async Task<Result<IEnumerable<BookResponse>>> HandleRequest(GetBooksQuery request, CancellationToken cancellationToken)
    {
        List<Book> books = await applicationUnitOfWork.Books.ToListAsync(cancellationToken).ConfigureAwait(false);

        return Result<IEnumerable<BookResponse>>.Success(books.Select(book => book.ToResponse()));
    }
}
