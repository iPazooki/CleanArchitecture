namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

/// <summary>
/// Handles the GetBookQuery to retrieve a book.
/// </summary>
public class GetBookQueryHandler(IApplicationUnitOfWork applicationUnitOfWork) : IRequestHandler<GetBookQuery, Result<BookDto>>
{
    /// <summary>
    /// Handles the GetBookQuery request.
    /// </summary>
    /// <param name="request">The GetBookQuery request containing the book ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Result with BookDto.</returns>
    public async Task<Result<BookDto>> Handle(GetBookQuery request, CancellationToken cancellationToken)
    {
        // Retrieve the book from the database using the provided ID.
        Book? book = await applicationUnitOfWork.Books.FindAsync(keyValues: [request.Id], cancellationToken);

        // Return a failure result if the book is not found, otherwise return a success result with the mapped BookDto.
        return book is null ? Result<BookDto>.Failure("Book Not Found.") : Result<BookDto>.Success(book.MapBook());
    }
}