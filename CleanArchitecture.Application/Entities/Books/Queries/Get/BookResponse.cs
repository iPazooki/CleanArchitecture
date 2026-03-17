namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

/// <summary>
/// Data Transfer Object for a Book.
/// </summary>
/// <param name="Id">The unique identifier of the book.</param>
/// <param name="Title">The title of the book.</param>
/// <param name="Genre">The genre of the book.</param>
public record BookResponse(Guid Id, string Title, string Genre);
