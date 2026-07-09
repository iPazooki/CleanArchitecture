namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

internal sealed class GetBooksQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetBooksQuery, Result<PaginatedResponse<BookResponse>>>
{
    public async ValueTask<Result<PaginatedResponse<BookResponse>>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        int totalCount = await dbContext.Books
            .AsNoTracking()
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        // Title alone is not a total order, so books sharing a title could repeat or vanish
        // across page boundaries. Id breaks the tie and makes paging deterministic.
        List<BookResponse> items = await dbContext.Books
            .AsNoTracking()
            .OrderBy(book => book.Title)
            .ThenBy(book => book.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(BookMappings.Projection)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        PaginatedResponse<BookResponse> response = new(items, request.Page, request.PageSize, totalCount);

        return Result<PaginatedResponse<BookResponse>>.Success(response);
    }
}
