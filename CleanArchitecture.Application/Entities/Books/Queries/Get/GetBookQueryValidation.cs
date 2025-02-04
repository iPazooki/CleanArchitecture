namespace CleanArchitecture.Application.Entities.Books.Queries.Get;

/// <summary>
/// Validator for the GetBookQuery.
/// </summary>
public class GetBookQueryValidation : AbstractValidator<GetBookQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetBookQueryValidation"/> class.
    /// </summary>
    public GetBookQueryValidation()
    {
        // Rule to ensure the Id property is not empty and provides a custom error message.
        RuleFor(b => b.Id)
            .NotEmpty()
            .WithMessage("this field is Required");
    }
}