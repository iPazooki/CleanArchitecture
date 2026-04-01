namespace CleanArchitecture.Application.Entities.Books.Commands.Update;

/// <summary>
/// Validator for the UpdateBookCommand.
/// </summary>
public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBookCommandValidator"/> class.
    /// </summary>
    public UpdateBookCommandValidator()
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
