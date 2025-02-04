namespace CleanArchitecture.Application.Entities.Books.Commands.Create;

/// <summary>
/// Handles the creation of a new book.
/// </summary>
/// <param name="applicationUnitOfWork">The unit of work to interact with the application's data.</param>
public class CreateBookCommandHandler(IApplicationUnitOfWork applicationUnitOfWork)
    : IRequestHandler<CreateBookCommand, Result<int>>
{
    /// <summary>
    /// Handles the create book command.
    /// </summary>
    /// <param name="request">The create book command containing the book details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ID of the created book.</returns>
    public async Task<Result<int>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // Creates a new Book entity with the provided title and genre.
        Book book = new()
        {
            Title = request.Title,
            Genre = Genre.FromCode(request.Genre)
        };

        // Adds the new book to the unit of work.
        applicationUnitOfWork.Books.Add(book);

        // Saves the changes asynchronously and gets the result.
        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken);
        
        // Returns the result, including the ID of the created book if successful.
        return result.IsSuccess ? Result<int>.Success(book.Id) : Result<int>.Failure("An error occurred while creating the book.");
    }
}