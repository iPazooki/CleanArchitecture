namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

internal class GetBooksQueryHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<GetBooksQuery>> validators)
    : BaseRequestHandler<GetBooksQuery, PaginatedResponse<BookResponse>>(validators)
{
    private const int MaxPageSize = 100;

    protected override async Task<Result<PaginatedResponse<BookResponse>>> HandleRequest(GetBooksQuery request, CancellationToken cancellationToken)
    {
        int page = Math.Max(1, request.Page);
        int pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        int totalCount = await applicationUnitOfWork.Books
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        IReadOnlyList<Book> books = await applicationUnitOfWork.Books
            .GetPagedAsync((page - 1) * pageSize, pageSize, cancellationToken)
            .ConfigureAwait(false);

        List<BookResponse> items = books
            .Select(book => new BookResponse(book.Id, book.Title, book.Genre))
            .ToList();

        PaginatedResponse<BookResponse> response = new(items, page, pageSize, totalCount);

        return Result<PaginatedResponse<BookResponse>>.Success(response);
    }
}