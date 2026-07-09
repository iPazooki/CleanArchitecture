using CleanArchitecture.Domain.Validations;

namespace CleanArchitecture.Application.Entities.Books.Commands;

/// <summary>
/// Shared FluentValidation rules for book commands, so the Title/Genre constraints have a single
/// source of truth across create and update.
/// </summary>
internal static class BookValidationRules
{
    /// <summary>Applies the standard title rules (required, min/max length) using the domain's <see cref="BookRules"/>.</summary>
    public static IRuleBuilderOptions<T, string> ValidBookTitle<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .NotEmpty().WithMessage(BookErrors.TitleIsRequired.Message)
            .MinimumLength(BookRules.TitleMinLength).WithMessage(BookErrors.TitleIsTooShort.Message)
            .MaximumLength(BookRules.TitleMaxLength).WithMessage(BookErrors.TitleTooLong.Message);

    /// <summary>Applies the standard genre rules (required, exact length range) using the domain's <see cref="BookRules"/>.</summary>
    public static IRuleBuilderOptions<T, string> ValidBookGenre<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder
            .NotEmpty().WithMessage(BookErrors.GenreIsRequired.Message)
            .Length(BookRules.GenreMinLength, BookRules.GenreMaxLength).WithMessage(BookErrors.GenreIsInvalidLength.Message);
}
