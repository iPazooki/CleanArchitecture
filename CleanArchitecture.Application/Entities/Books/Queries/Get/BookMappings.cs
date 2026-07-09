using System.Linq.Expressions;

namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

/// <summary>
/// Provides a single, EF Core-translatable projection from <see cref="Book"/> to <see cref="BookResponse"/>,
/// shared by every query so the mapping has one source of truth.
/// </summary>
public static class BookMappings
{
    /// <summary>
    /// Projects a <see cref="Book"/> onto a <see cref="BookResponse"/>. Usable both in EF Core
    /// <c>Select</c> calls (translated to SQL) and in memory via <see cref="ToResponse"/>.
    /// </summary>
    public static Expression<Func<Book, BookResponse>> Projection { get; } =
        book => new BookResponse(book.Id, book.Title, book.Genre.Code);

    private static readonly Func<Book, BookResponse> _compiledProjection = Projection.Compile();

    /// <summary>
    /// Maps a materialized <see cref="Book"/> entity to a <see cref="BookResponse"/>.
    /// </summary>
    /// <param name="book">The book entity to map.</param>
    /// <returns>The mapped <see cref="BookResponse"/>.</returns>
    public static BookResponse ToResponse(this Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        return _compiledProjection(book);
    }
}
