namespace CleanArchitecture.Application.Entities.Login.Commands;

public class LoginCommandValidations : AbstractValidator<LoginCommand>
{
    public LoginCommandValidations()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(string.Format(GeneralErrors.RequiredFieldErrorMessage, "Email"))
            .EmailAddress().WithMessage("Email is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(string.Format(GeneralErrors.RequiredFieldErrorMessage, "Password"))
            .MinimumLength(Constants.MinPasswordLength).WithMessage($"Password must be at least {Constants.MinPasswordLength} characters long.");
    }
}