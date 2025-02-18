using CleanArchitecture.Domain.Entities.Book;

namespace CleanArchitecture.Application.Entities.Books.Commands.Update;

/// <summary>
/// Handles the update of a book.
/// </summary>
/// <param name="applicationUnitOfWork">The unit of work to interact with the application's data.</param>
public class UpdateBookCommandHandler(IApplicationUnitOfWork applicationUnitOfWork)
    : IRequestHandler<UpdateBookCommand, Result>
{
    /// <summary>
    /// Handles the update book command.
    /// </summary>
    /// <param name="request">The update book command containing the book details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
    public async Task<Result> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        // Finds the book by ID asynchronously.
        Book? book = await applicationUnitOfWork.Books.FindAsync(keyValues: [request.Id], cancellationToken);

        // Returns a failure result if the book is not found.
        if (book is null)
        {
            return Result.Failure("Book Not Found.");
        }

        // Updates the book entity with the details from the request.
        book.UpdateFromRequest(request);

        // Saves the changes asynchronously and returns the result.
        return await applicationUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}