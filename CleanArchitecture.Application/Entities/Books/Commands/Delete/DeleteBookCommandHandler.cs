namespace CleanArchitecture.Application.Entities.Books.Commands.Delete;

/// <summary>
/// Handles the deletion of a book.
/// </summary>
/// <param name="applicationUnitOfWork">The unit of work to interact with the application's data.</param>
public class DeleteBookCommandHandler(IApplicationUnitOfWork applicationUnitOfWork)
    : IRequestHandler<DeleteBookCommand, Result>
{
    /// <summary>
    /// Handles the delete book command.
    /// </summary>
    /// <param name="request">The delete book command containing the book ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
    public async Task<Result> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        // Finds the book by ID asynchronously.
        Book? book = await applicationUnitOfWork.Books.FindAsync(keyValues: [request.Id], cancellationToken);

        // Returns a failure result if the book is not found.
        if (book is null)
        {
            return Result.Failure("Book Not Found.");
        }

        // Removes the book from the unit of work.
        applicationUnitOfWork.Books.Remove(book);

        // Saves the changes asynchronously and returns the result.
        return await applicationUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}