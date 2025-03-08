using CleanArchitecture.Domain.Entities.Book;

namespace CleanArchitecture.Application.Entities.Books.Commands.Update;

internal class UpdateBookRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<UpdateBookCommand>> validators)
    : BaseRequestHandler<UpdateBookCommand>(validators)
{
    protected override async Task<Result> HandleRequest(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        Book? book = await applicationUnitOfWork.Books.FindAsync(keyValues: [request.Id], cancellationToken);
        
        if (book is null)
        {
            return Result.Failure("Book Not Found.");
        }
        
        book.UpdateFromRequest(request);
        
        return await applicationUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}