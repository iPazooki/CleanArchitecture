using CleanArchitecture.Domain.Validations.Book;

namespace CleanArchitecture.Application.Entities.Books.Commands.Update;

internal sealed class UpdateBookCommandHandler(IApplicationUnitOfWork applicationUnitOfWork)
    : IRequestHandler<UpdateBookCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        Book? book = await applicationUnitOfWork.Books.FindAsync(keyValues: [request.Id], cancellationToken).ConfigureAwait(false);

        if (book is null)
        {
            return Result.Failure(BookErrors.BookNotFound);
        }

        Result<Genre> genreResult = Genre.FromCode(request.Genre);
        if (!genreResult.IsSuccess)
        {
            return Result.Failure(genreResult.Errors.ToArray());
        }

        Result updateResult = book.Update(request.Title, genreResult.Value!);

        if (!updateResult.IsSuccess)
        {
            return updateResult;
        }

        return await applicationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
