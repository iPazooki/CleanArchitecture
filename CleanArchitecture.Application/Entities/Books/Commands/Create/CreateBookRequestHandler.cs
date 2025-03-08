using CleanArchitecture.Domain.Entities.Book;

namespace CleanArchitecture.Application.Entities.Books.Commands.Create;

internal class CreateBookRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<CreateBookCommand>> validators)
    : BaseRequestHandler<CreateBookCommand, Guid>(validators)
{
    protected override async Task<Result<Guid>> HandleRequest(CreateBookCommand request,
        CancellationToken cancellationToken)
    {
        Result<Book> book = Book.Create(request.Title, Genre.FromCode(request.Genre));

        if (!book.IsSuccess)
        {
            return Result<Guid>.Failure(book.Errors.ToArray());
        }
        
        applicationUnitOfWork.Books.Add(book!);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken);
        
        return result.IsSuccess
            ? Result<Guid>.Success(book.Value!.Id)
            : Result<Guid>.Failure(string.Format(GeneralErrors.GeneralCreateErrorMessage, nameof(Book)));
    }
}