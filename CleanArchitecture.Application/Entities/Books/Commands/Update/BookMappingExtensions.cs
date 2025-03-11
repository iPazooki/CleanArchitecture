using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Entities.Books.Commands.Update;

/// <summary>
/// Provides extension methods for mapping book update commands to book entities.
/// </summary>
public static class BookMappingExtensions
{
    /// <summary>
    /// Updates the book entity with the details from the update book command.
    /// </summary>
    /// <param name="book">The book entity to be updated.</param>
    /// <param name="request">The update book command containing the new book details.</param>
    public static void UpdateFromRequest(this Book book, UpdateBookCommand request)
    {
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(request);
        
        book.Title = request.Title;
        book.Genre = Genre.FromCode(request.Genre);
    }
}
