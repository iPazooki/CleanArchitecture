namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

/// <summary>
/// Get books query with pagination support.
/// </summary>
/// <param name="Page">The page number (1-based). Defaults to 1.</param>
/// <param name="PageSize">The number of items per page. Defaults to 10, max 100.</param>
public record GetBooksQuery(int Page = 1, int PageSize = 10) : IRequest<Result<PaginatedResponse<BookResponse>>>;