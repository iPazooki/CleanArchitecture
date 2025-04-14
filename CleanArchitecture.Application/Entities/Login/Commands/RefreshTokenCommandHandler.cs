using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Entities.Login.Commands;

internal class RefreshTokenCommandHandler(IApplicationUnitOfWork unitOfWork, IJwtTokenProvider jwtTokenProvider, IEnumerable<IValidator<RefreshTokenCommand>> validators)
    : BaseRequestHandler<RefreshTokenCommand, JwtTokenResponse>(validators)
{
    protected override async Task<Result<JwtTokenResponse>> HandleRequest(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        RefreshToken? refreshToken = await GetRefreshTokenAsync(request.RefreshToken, cancellationToken).ConfigureAwait(false);

        if (refreshToken is null)
        {
            return Result<JwtTokenResponse>.Failure("Invalid refresh token");
        }

        if (refreshToken.ExpiresDate < DateTimeOffset.UtcNow)
        {
            return Result<JwtTokenResponse>.Failure("Refresh token expired");
        }

        InvalidateRefreshToken(refreshToken);

        (string jwtToken, DateTimeOffset jwtExpirationDate) = await jwtTokenProvider.GenerateJwtTokenAsync(refreshToken.User).ConfigureAwait(false);

        Result<RefreshToken> newRefreshToken = CreateRefreshToken(refreshToken.User);

        if (!newRefreshToken.IsSuccess)
        {
            return Result<JwtTokenResponse>.Failure(newRefreshToken.Errors.ToArray());
        }

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new JwtTokenResponse(jwtToken, newRefreshToken.Value!.Token, jwtExpirationDate);
    }

    private async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await unitOfWork.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.Token == token, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    private void InvalidateRefreshToken(RefreshToken refreshToken)
    {
        unitOfWork.RefreshTokens.Remove(refreshToken);
    }

    private Result<RefreshToken> CreateRefreshToken(User user)
    {
        (string refreshToken, DateTimeOffset refreshTokenExpirationDate) = jwtTokenProvider.GenerateRefreshToken();

        Result<RefreshToken> refreshTokenEntity = RefreshToken.Create(refreshToken, user, refreshTokenExpirationDate);

        if (!refreshTokenEntity.IsSuccess)
        {
            return Result<RefreshToken>.Failure(refreshTokenEntity.Errors.ToArray());
        }

        unitOfWork.RefreshTokens.Add(refreshTokenEntity.Value!);

        return refreshTokenEntity;
    }
}
