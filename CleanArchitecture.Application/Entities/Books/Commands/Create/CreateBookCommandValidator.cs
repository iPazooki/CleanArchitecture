using CleanArchitecture.Domain.Validations.Book;

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
            .NotEmpty().WithMessage(BookErrors.TitleIsRequired.Message)
            .MinimumLength(BookRules.TitleMinLength).WithMessage(BookErrors.TitleIsTooShort.Message)
            .MaximumLength(BookRules.TitleMaxLength).WithMessage(BookErrors.TitleTooLong.Message);

        RuleFor(v => v.Genre)
            .NotEmpty().WithMessage(BookErrors.GenreIsRequired.Message)
            .Length(BookRules.GenreMinLength, BookRules.GenreMaxLength).WithMessage(BookErrors.GenreIsInvalidLength.Message);
    }
}
