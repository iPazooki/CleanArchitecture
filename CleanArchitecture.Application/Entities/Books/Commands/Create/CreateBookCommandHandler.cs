using System.Text;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Entities.Books.Commands.Create;

internal class CreateBookCommandHandler(IApplicationUnitOfWork applicationUnitOfWork, IEnumerable<IValidator<CreateBookCommand>> validators)
    : BaseRequestHandler<CreateBookCommand, Guid>(validators)
{
    private static readonly CompositeFormat _errorMessage = CompositeFormat.Parse(GeneralErrors.GeneralCreateErrorMessage);

    protected override async Task<Result<Guid>> HandleRequest(CreateBookCommand request, CancellationToken cancellationToken)
    {
        Result<Book> book = Book.Create(request.Title, Genre.FromCode(request.Genre));

        if (!book.IsSuccess)
        {
            return Result<Guid>.Failure(book.Errors.ToArray());
        }

        await applicationUnitOfWork.Books.AddAsync(book!, cancellationToken).ConfigureAwait(false);

        Result result = await applicationUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return result.IsSuccess
            ? Result<Guid>.Success(book.Value!.Id)
            : Result<Guid>.Failure(string.Format(CultureInfo.InvariantCulture, _errorMessage, nameof(Book)));
    }
}
