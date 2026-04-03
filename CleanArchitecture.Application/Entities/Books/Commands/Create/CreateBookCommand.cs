namespace CleanArchitecture.Application.Entities.Books.Commands.Create;

/// <summary>
/// Represents a command to create a new book.
/// </summary>
/// <param name="Title">The title of the book.</param>
/// <param name="Genre">The genre of the book.</param>
/// <returns>The ID of the created book.</returns>
public record CreateBookCommand(string Title, string Genre) : IRequest<Result<Guid>>;