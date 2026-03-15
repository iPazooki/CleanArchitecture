using CleanArchitecture.Domain.Events.Book;

namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// Represents a book entity as an immutable aggregate root following DDD principles.
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
        // Domain validation
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Book>.Failure(BookErrors.TitleIsRequired);
        }

        if (title.Length > 200)
        {
            return Result<Book>.Failure(BookErrors.TitleTooLong);
        }

        Book book = new(title, genre);

        book.AddDomainEvent(new BookAddedEvent(book));

        return Result<Book>.Success(book);
    }

    /// <summary>
    /// Updates the title of the book with domain validation.
    /// </summary>
    /// <param name="newTitle">The new title.</param>
    /// <returns>A Result indicating success or validation errors.</returns>
    public Result UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            return Result.Failure(BookErrors.TitleIsRequired);
        }

        if (newTitle.Length > 200)
        {
            return Result.Failure(BookErrors.TitleTooLong);
        }

        Title = newTitle;
        return Result.Success();
    }

    /// <summary>
    /// Updates the genre of the book.
    /// </summary>
    /// <param name="newGenre">The new genre.</param>
    public void UpdateGenre(Genre newGenre)
    {
        Genre = newGenre;
    }
}
