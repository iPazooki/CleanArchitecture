using CleanArchitecture.Domain.Validations;

namespace CleanArchitecture.Application.Entities.Books.Commands.Delete;

internal sealed class DeleteBookCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<DeleteBookCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        Book? book = await dbContext.Books.FindAsync(keyValues: [request.Id], cancellationToken).ConfigureAwait(false);

        if (book is null)
        {
            return Result.Failure(BookErrors.BookNotFound);
        }

        dbContext.Books.Remove(book);

        return await dbContext.SaveEntitiesAsync(cancellationToken).ConfigureAwait(false);
    }
}
