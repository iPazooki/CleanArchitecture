namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

/// <summary>
/// Provides extension methods for mapping Book entities to BookDto.
/// </summary>
public static class BookMappingExtensions
{
    /// <summary>
    /// Maps a Book entity to a BookDto.
    /// </summary>
    /// <param name="book">The Book entity to map.</param>
    /// <returns>A BookDto containing the mapped data.</returns>
    public static BookDto MapBook(this Book book) => new(book.Title, book.Genre);
}