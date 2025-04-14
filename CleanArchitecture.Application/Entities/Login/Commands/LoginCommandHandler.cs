using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Entities.Login.Commands;

internal class LoginCommandHandler(IApplicationUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenProvider jwtTokenProvider, IEnumerable<IValidator<LoginCommand>> validators)
    : BaseRequestHandler<LoginCommand, JwtTokenResponse>(validators)
{
    protected override async Task<Result<JwtTokenResponse>> HandleRequest(LoginCommand request, CancellationToken cancellationToken)
    {
        User? user = await GetValidUserAsync(request, cancellationToken).ConfigureAwait(false);

        if (user is null)
        {
            return Result<JwtTokenResponse>.Failure(SecurityErrors.EmailOrPasswordIncorrect);
        }

        (string jwtToken, DateTimeOffset jwtExpirationDate) = await jwtTokenProvider.GenerateJwtTokenAsync(user).ConfigureAwait(false);

        Result<RefreshToken> refreshToken = await CreateAndStoreRefreshTokenAsync(user, cancellationToken).ConfigureAwait(false);

        if (!refreshToken.IsSuccess)
        {
            return Result<JwtTokenResponse>.Failure(refreshToken.Errors.ToArray());
        }

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new JwtTokenResponse(jwtToken, refreshToken.Value!.Token, jwtExpirationDate);
    }

    private async Task<User?> GetValidUserAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        User? user = await unitOfWork.Users
            .SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (user is null || !passwordHasher.VerifyPassword(request.Password, user.HashedPassword))
        {
            return null;
        }

        return user;
    }

    private async Task<Result<RefreshToken>> CreateAndStoreRefreshTokenAsync(User user, CancellationToken cancellationToken)
    {
        (string refreshToken, DateTimeOffset refreshTokenExpirationDate) = jwtTokenProvider.GenerateRefreshToken();

        Result<RefreshToken> refreshTokenEntity = RefreshToken.Create(refreshToken, user, refreshTokenExpirationDate);

        if (!refreshTokenEntity.IsSuccess)
        {
            return Result<RefreshToken>.Failure(refreshTokenEntity.Errors.ToArray());
        }

        await unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity.Value!, cancellationToken).ConfigureAwait(false);

        return refreshTokenEntity;
    }
}
