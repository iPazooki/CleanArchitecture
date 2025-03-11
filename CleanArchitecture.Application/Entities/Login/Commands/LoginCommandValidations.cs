using System.Text;

namespace CleanArchitecture.Application.Entities.Login.Commands;

public class LoginCommandValidations : AbstractValidator<LoginCommand>
{
    private static readonly CompositeFormat _errorMessage = CompositeFormat.Parse(GeneralErrors.RequiredFieldErrorMessage);

    public LoginCommandValidations()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(string.Format(CultureInfo.InvariantCulture, _errorMessage, "Email"))
            .EmailAddress().WithMessage("Email is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(string.Format(CultureInfo.InvariantCulture, _errorMessage, "Password"))
            .MinimumLength(Constants.MinPasswordLength)
            .WithMessage($"Password must be at least {Constants.MinPasswordLength} characters long.");
    }
}
