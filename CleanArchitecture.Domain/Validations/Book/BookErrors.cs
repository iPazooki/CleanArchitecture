using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Validations.Book;

/// <summary>
/// Contains all domain errors related to the Book aggregate.
/// </summary>
public static class BookErrors
{
    public static readonly DomainError TitleIsRequired = DomainError.Validation(
        "Book.TitleIsRequired",
        Resources.BookErrors.Book_TitleIsRequired);

    public static readonly DomainError TitleTooLong = DomainError.Validation(
        "Book.TitleTooLong",
        Resources.BookErrors.Book_TitleTooLong);

    public static readonly DomainError InvalidGenre = DomainError.Validation(
        "Book.InvalidGenre",
        Resources.BookErrors.Book_InvalidGenre);

    public static readonly DomainError BookNotFound = DomainError.NotFound(
        "Book.NotFound",
        Resources.BookErrors.Book_NotFound);

    public static readonly DomainError TitleIsTooShort = DomainError.Validation(
        "Book.TitleIsTooShort",
        Resources.BookErrors.Book_TitleIsTooShort);

    public static readonly DomainError GenreIsRequired = DomainError.Validation(
        "Book.GenreIsRequired",
        Resources.BookErrors.Book_GenreIsRequired);

    public static readonly DomainError GenreIsInvalidLength = DomainError.Validation(
        "Book.GenreIsInvalidLength",
        Resources.BookErrors.Book_GenreIsInvalidLength);
}
