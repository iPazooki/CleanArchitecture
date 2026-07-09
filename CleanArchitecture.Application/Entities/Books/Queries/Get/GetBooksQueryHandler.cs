namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

internal sealed class GetBooksQueryHandler(IApplicationUnitOfWork applicationUnitOfWork)
    : IRequestHandler<GetBooksQuery, Result<PaginatedResponse<BookResponse>>>
{
    public async ValueTask<Result<PaginatedResponse<BookResponse>>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        int totalCount = await applicationUnitOfWork.Books
            .AsNoTracking()
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

        List<BookResponse> items = await applicationUnitOfWork.Books
            .AsNoTracking()
            .OrderBy(book => book.Title)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(BookMappings.Projection)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        PaginatedResponse<BookResponse> response = new(items, request.Page, request.PageSize, totalCount);

        return Result<PaginatedResponse<BookResponse>>.Success(response);
    }
}
