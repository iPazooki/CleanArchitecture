namespace CleanArchitecture.Domain.Validations;

/// <summary>
/// Contains all domain errors related to the Book aggregate.
/// </summary>
/// <remarks>
/// These are expression-bodied properties, not <c>static readonly</c> fields, on purpose:
/// each access re-reads the RESX through the ambient <see cref="System.Globalization.CultureInfo"/>,
/// which is what lets <c>UseRequestLocalization</c> return messages in the caller's language.
/// Caching them would freeze every message in the culture that happened to start the host.
/// </remarks>
public static class BookErrors
{
    public static DomainError TitleIsRequired => DomainError.Validation(
        "Book.TitleIsRequired",
        Resources.BookErrors.Book_TitleIsRequired);

    public static DomainError TitleTooLong => DomainError.Validation(
        "Book.TitleTooLong",
        Resources.BookErrors.Book_TitleTooLong);

    public static DomainError InvalidGenre => DomainError.Validation(
        "Book.InvalidGenre",
        Resources.BookErrors.Book_InvalidGenre);

    public static DomainError BookNotFound => DomainError.NotFound(
        "Book.NotFound",
        Resources.BookErrors.Book_NotFound);

    public static DomainError TitleIsTooShort => DomainError.Validation(
        "Book.TitleIsTooShort",
        Resources.BookErrors.Book_TitleIsTooShort);

    public static DomainError GenreIsRequired => DomainError.Validation(
        "Book.GenreIsRequired",
        Resources.BookErrors.Book_GenreIsRequired);

    public static DomainError GenreIsInvalidLength => DomainError.Validation(
        "Book.GenreIsInvalidLength",
        Resources.BookErrors.Book_GenreIsInvalidLength);
}
