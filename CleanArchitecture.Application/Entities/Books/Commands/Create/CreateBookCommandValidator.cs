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
        // Rule to ensure the Title property is not empty and has a maximum length of 200 characters.
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();

        // Rule to ensure the Genre property is not empty.
        RuleFor(v => v.Genre)
            .NotEmpty();
    }
}