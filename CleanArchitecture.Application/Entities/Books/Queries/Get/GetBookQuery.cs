namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

/// <summary>
/// Represents a query to get a book by its ID.
/// </summary>
/// <param name="Id">The ID of the book to retrieve.</param>
public record GetBookQuery(int Id) : IRequest<Result<BookResponse>>;