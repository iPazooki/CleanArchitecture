namespace CleanArchitecture.Application.Entities.Books.Commands.Update;

/// <summary>
/// Validator for the UpdateBookCommand.
/// </summary>
internal sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBookCommandValidator"/> class.
    /// </summary>
    public UpdateBookCommandValidator()
    {
        RuleFor(v => v.Title).ValidBookTitle();
        RuleFor(v => v.Genre).ValidBookGenre();
    }
}
