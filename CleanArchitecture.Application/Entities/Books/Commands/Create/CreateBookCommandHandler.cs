using System.Text;

namespace CleanArchitecture.Application.Entities.Books.Commands.Create;

internal sealed class CreateBookCommandHandler(IApplicationUnitOfWork applicationUnitOfWork)
    : IRequestHandler<CreateBookCommand, Result<Guid>>
{
    private static readonly CompositeFormat _errorMessage = CompositeFormat.Parse(GeneralErrors.GeneralCreateErrorMessage);

    public async ValueTask<Result<Guid>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        Result<Genre> genreResult = Genre.FromCode(request.Genre);

        if (!genreResult.IsSuccess)
        {
            return Result<Guid>.Failure(genreResult.Errors.ToArray());
        }

        Result<Book> book = Book.Create(request.Title, genreResult.Value!);

        if (!book.IsSuccess)
        {
            return Result<Guid>.Failure(book.Errors.ToArray());
        }

        await applicationUnitOfWork.Books.AddAsync(book.Value!, cancellationToken).ConfigureAwait(false);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result<Guid>.Success(book.Value!.Id)
            : Result<Guid>.Failure(string.Format(CultureInfo.InvariantCulture, _errorMessage, nameof(Book)));
    }
}
