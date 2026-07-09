namespace CleanArchitecture.Application.Entities.Books.Commands.Create;

internal sealed class CreateBookCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateBookCommand, Result<Guid>>
{
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

        await dbContext.Books.AddAsync(book.Value!, cancellationToken).ConfigureAwait(false);

        Result result = await dbContext.SaveEntitiesAsync(cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result<Guid>.Success(book.Value!.Id)
            : Result<Guid>.Failure(result.Errors.ToArray());
    }
}
