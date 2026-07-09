using System.Text;

namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

/// <summary>
/// Validator for the <see cref="GetBookQuery"/>.
/// </summary>
public class GetBookQueryValidator : AbstractValidator<GetBookQuery>
{
    private static readonly CompositeFormat _requiredField = CompositeFormat.Parse(GeneralErrors.RequiredFieldErrorMessage);

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBookQueryValidator"/> class.
    /// </summary>
    public GetBookQueryValidator()
    {
        RuleFor(b => b.Id)
            .NotEmpty()
            .WithMessage(string.Format(CultureInfo.InvariantCulture, _requiredField, nameof(GetBookQuery.Id)));
    }
}
