using CleanArchitecture.Domain.Entities.Book;

namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

/// <summary>
/// Provides extension methods for mapping Book entities to BookDto.
/// </summary>
public static class BookExtensions
{
    /// <summary>
    /// Maps a Book entity to a BookDto.
    /// </summary>
    /// <param name="book">The Book entity to map.</param>
    /// <returns>A BookDto containing the mapped data.</returns>
    public static BookResponse ToResponse(this Book book) => new(book.Title, book.Genre);
}