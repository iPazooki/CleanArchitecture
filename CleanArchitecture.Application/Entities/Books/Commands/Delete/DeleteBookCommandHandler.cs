using CleanArchitecture.Domain.Validations.Book;

namespace CleanArchitecture.Application.Entities.Books.Commands.Delete;

internal sealed class DeleteBookCommandHandler(IApplicationUnitOfWork applicationUnitOfWork)
    : IRequestHandler<DeleteBookCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        Book? book = await applicationUnitOfWork.Books.FindAsync(keyValues: [request.Id], cancellationToken).ConfigureAwait(false);

        if (book is null)
        {
            return Result.Failure(BookErrors.BookNotFound);
        }

        applicationUnitOfWork.Books.Remove(book);

        return await applicationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
