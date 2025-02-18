namespace CleanArchitecture.Application.Entities.Persons.Commands.Create;

public class CreatePersonCommandValidation : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidation()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required.")
            .MaximumLength(50).WithMessage("FirstName must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required.")
            .MaximumLength(50).WithMessage("LastName must not exceed 50 characters.");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address is required.");

        RuleFor(x => x.Gender)
            .Must(gender => Enum.IsDefined(typeof(Gender), gender))
            .WithMessage("Invalid Gender.");
    }
}