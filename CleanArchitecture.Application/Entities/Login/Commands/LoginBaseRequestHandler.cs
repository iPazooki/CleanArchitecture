using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Entities.Login.Commands;

internal class LoginBaseRequestHandler(IApplicationUnitOfWork applicationUnitOfWork, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, IEnumerable<IValidator<LoginCommand>> validators)
    : BaseRequestHandler<LoginCommand, string>(validators)
{
    protected override async Task<Result<string>> HandleRequest(LoginCommand request, CancellationToken cancellationToken)
    {
        User? user = await applicationUnitOfWork.Users.SingleOrDefaultAsync(u => u.Email == request.Email,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (user is null || !passwordHasher.VerifyPassword(request.Password, user.HashedPassword))
        {
            return Result<string>.Failure(SecurityErrors.EmailOrPasswordIncorrect);
        }

        return await jwtProvider.GenerateJwtTokenAsync(user).ConfigureAwait(false);
    }
}
