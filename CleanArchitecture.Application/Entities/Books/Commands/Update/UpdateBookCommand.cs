namespace CleanArchitecture.Application.Entities.Books.Commands.Update;

/// <summary>
/// Represents a command to update a book.
/// </summary>
/// <param name="Id">The ID of the book to be updated.</param>
/// <param name="Title">The new title of the book.</param>
/// <param name="Genre">The new genre of the book.</param>
public record UpdateBookCommand(Guid Id, string Title, string Genre) : IRequest<Result>;