namespace CleanArchitecture.Application.Entities.Books.Commands.Create;

/// <summary>
/// Validator for the CreateBookCommand.
/// </summary>
internal sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateBookCommandValidator"/> class.
    /// </summary>
    public CreateBookCommandValidator()
    {
        RuleFor(v => v.Title).ValidBookTitle();
        RuleFor(v => v.Genre).ValidBookGenre();
    }
}
