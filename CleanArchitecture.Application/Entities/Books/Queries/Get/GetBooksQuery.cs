namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

/// <summary>
/// Get books query
/// </summary>
public record GetBooksQuery : IRequest<Result<IEnumerable<BookResponse>>>;
