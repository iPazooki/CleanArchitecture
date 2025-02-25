using CleanArchitecture.Domain.Entities.User;

namespace CleanArchitecture.Application.Entities.Login.Commands;

public class LoginCommandHandler(IApplicationUnitOfWork applicationUnitOfWork, IPasswordHasher passwordHasher, IJwtProvider jwtProvider) : IRequestHandler<LoginCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User? user = await applicationUnitOfWork.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken: cancellationToken);

        if (user is null || !passwordHasher.VerifyPassword(request.Password, user.HashedPassword))
        {
            return Result<string>.Failure(SecurityErrors.EmailOrPasswordIncorrect);
        }

        return await jwtProvider.GenerateJwtTokenAsync(user);
    }
}