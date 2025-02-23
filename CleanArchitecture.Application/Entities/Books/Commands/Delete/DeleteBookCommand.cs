namespace CleanArchitecture.Application.Entities.Books.Commands.Delete;

/// <summary>
/// Represents a command to delete a book.
/// </summary>
/// <param name="Id">The ID of the book to be deleted.</param>
public record DeleteBookCommand(Guid Id) : IRequest<Result>;