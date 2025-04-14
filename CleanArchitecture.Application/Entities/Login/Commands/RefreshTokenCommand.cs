namespace CleanArchitecture.Application.Entities.Login.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<JwtTokenResponse>>;
