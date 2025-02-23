namespace CleanArchitecture.Application.Entities.Users.Commands.Create;

public class CreateUserCommandValidations : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidations()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required.")
            .MaximumLength(50).WithMessage("FirstName must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required.")
            .MaximumLength(50).WithMessage("LastName must not exceed 50 characters.");

        When(x => x.Gender is not null, () =>
        {
            RuleFor(x => x.Gender)
                .Must(gender => Enum.IsDefined(typeof(Gender), gender!))
                .WithMessage("Invalid Gender.");
        });
        
        When(x => !string.IsNullOrEmpty(x.Email), () =>
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("HashedPassword is required.")
                .MinimumLength(Constants.MinPasswordLength).WithMessage($"HashedPassword must not be less than {Constants.MinPasswordLength} characters.");
        });
    }
}