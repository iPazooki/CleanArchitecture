namespace CleanArchitecture.Application.Entities.Books.Commands.Create;

/// <summary>
/// Validator for the CreateBookCommand.
/// </summary>
public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateBookCommandValidator"/> class.
    /// </summary>
    public CreateBookCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(BookRules.TitleMinLength).WithMessage("Title is too short.")
            .MaximumLength(BookRules.TitleMaxLength).WithMessage("Title is too long.");

        RuleFor(v => v.Genre)
            .NotEmpty().WithMessage("Genre is required.")
            .Length(BookRules.GenreMinLength, BookRules.GenreMaxLength).WithMessage("Genre is of invalid length.");
    }
}
