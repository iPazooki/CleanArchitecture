using CleanArchitecture.Domain.Validations.Book;

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
            .NotEmpty().WithMessage(BookErrors.TitleIsRequired.Message)
            .MinimumLength(BookRules.TitleMinLength).WithMessage(BookErrors.TitleIsTooShort.Message)
            .MaximumLength(BookRules.TitleMaxLength).WithMessage(BookErrors.TitleTooLong.Message);

        RuleFor(v => v.Genre)
            .NotEmpty().WithMessage(BookErrors.GenreIsRequired.Message)
            .Length(BookRules.GenreMinLength, BookRules.GenreMaxLength).WithMessage(BookErrors.GenreIsInvalidLength.Message);
    }
}
