using CleanArchitecture.Domain.Events;
using CleanArchitecture.Domain.Validations;

namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// Represents a book aggregate root. State is mutated only through validated domain
/// behavior (<see cref="Create"/> and <see cref="Update"/>), following DDD principles.
/// </summary>
public sealed class Book : AggregateRootAuditable
{
    // Private constructor for EF Core
    private Book() { }

    // Private constructor for domain creation
    private Book(string title, Genre genre)
    {
        Title = title;
        Genre = genre;
    }

    /// <summary>
    /// Gets the title of the book.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the genre of the book.
    /// </summary>
    public Genre Genre { get; private set; } = default!;

    /// <summary>
    /// Factory method to create a new Book with domain validation.
    /// </summary>
    /// <param name="title">The title of the book.</param>
    /// <param name="genre">The genre of the book.</param>
    /// <returns>A Result containing the Book or validation errors.</returns>
    public static Result<Book> Create(string title, Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre);

        Result titleValidationResult = ValidateTitle(title);

        if (!titleValidationResult.IsSuccess)
        {
            return Result<Book>.Failure(titleValidationResult.Errors.ToArray());
        }

        Book book = new(title, genre);

        book.AddDomainEvent(new BookAddedEvent(book.Id));

        return Result<Book>.Success(book);
    }

    /// <summary>
    /// Updates the title and genre of the book with domain validation.
    /// </summary>
    /// <param name="newTitle">The new title.</param>
    /// <param name="newGenre">The new genre.</param>
    /// <returns>A Result indicating success or validation errors.</returns>
    public Result Update(string newTitle, Genre newGenre)
    {
        ArgumentNullException.ThrowIfNull(newGenre);

        Result titleValidationResult = ValidateTitle(newTitle);
        if (!titleValidationResult.IsSuccess)
        {
            return titleValidationResult;
        }

        Title = newTitle;
        Genre = newGenre;

        return Result.Success();
    }

    private static Result ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(BookErrors.TitleIsRequired);
        }

        if (title.Length < BookRules.TitleMinLength)
        {
            return Result.Failure(BookErrors.TitleIsTooShort);
        }

        if (title.Length > BookRules.TitleMaxLength)
        {
            return Result.Failure(BookErrors.TitleTooLong);
        }

        return Result.Success();
    }
}
