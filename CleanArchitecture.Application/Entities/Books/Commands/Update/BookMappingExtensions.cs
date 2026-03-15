using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Errors;

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
    /// <returns>A Result indicating success or validation errors.</returns>
    public static Result UpdateFromRequest(this Book book, UpdateBookCommand request)
    {
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(request);

        // Update title using domain method with validation
        Result titleUpdateResult = book.UpdateTitle(request.Title);
        if (!titleUpdateResult.IsSuccess)
        {
            return titleUpdateResult;
        }

        // Update genre
        Result<Genre> genreResult = Genre.FromCode(request.Genre);
        if (!genreResult.IsSuccess)
        {
            return Result.Failure(genreResult.Errors.ToArray());
        }

        book.UpdateGenre(genreResult.Value!);

        return Result.Success();
    }
}
