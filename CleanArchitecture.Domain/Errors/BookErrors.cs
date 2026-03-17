namespace CleanArchitecture.Domain.Errors;

/// <summary>
/// Contains all domain errors related to the Book aggregate.
/// </summary>
public static class BookErrors
{
    public static readonly DomainError TitleIsRequired = DomainError.Validation(
        "Book.TitleIsRequired",
        "Title is required and cannot be empty.");

    public static readonly DomainError TitleTooLong = DomainError.Validation(
        "Book.TitleTooLong",
        "Title cannot exceed 200 characters.");

    public static readonly DomainError InvalidGenre = DomainError.Validation(
        "Book.InvalidGenre",
        "The specified genre is not supported.");

    public static readonly DomainError BookNotFound = DomainError.NotFound(
        "Book.NotFound",
        "The book with the specified ID was not found.");
}