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
        // Rule to ensure the Title property is not empty and has a maximum length of 200 characters.
        RuleFor(v => v.Title)
            .MaximumLength(200)
            .NotEmpty();

        // Rule to ensure the Genre property is not empty.
        RuleFor(v => v.Genre)
            .NotEmpty();
    }
}