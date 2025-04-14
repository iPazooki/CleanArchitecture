using System.Text;

namespace CleanArchitecture.Application.Entities.Login.Commands;

public class RefreshTokenCommandValidations : AbstractValidator<RefreshTokenCommand>
{
    private static readonly CompositeFormat _errorMessage = CompositeFormat.Parse(GeneralErrors.RequiredFieldErrorMessage);

    public RefreshTokenCommandValidations()
    {
        RuleFor(x => x.ToString())
            .NotEmpty()
            .WithMessage(string.Format(CultureInfo.InvariantCulture, _errorMessage, nameof(RefreshTokenCommand.RefreshToken)));
    }
}
