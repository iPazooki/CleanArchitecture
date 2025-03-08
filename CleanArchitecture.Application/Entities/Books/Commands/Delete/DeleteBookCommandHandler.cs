using CleanArchitecture.Domain.Entities.Book;

namespace CleanArchitecture.Application.Entities.Books.Commands.Delete;

internal class DeleteBookRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<DeleteBookCommand>> validators)
    : BaseRequestHandler<DeleteBookCommand>(validators)
{
    protected override async Task<Result> HandleRequest(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        Book? book = await applicationUnitOfWork.Books.FindAsync(keyValues: [request.Id], cancellationToken);
        
        if (book is null)
        {
            return Result.Failure("Book Not Found.");
        }
        
        applicationUnitOfWork.Books.Remove(book);
        
        return await applicationUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}