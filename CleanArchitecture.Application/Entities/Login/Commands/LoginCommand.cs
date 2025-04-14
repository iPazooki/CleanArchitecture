namespace CleanArchitecture.Application.Entities.Login.Commands;


public record LoginCommand(string Email, string Password) : IRequest<Result<JwtTokenResponse>>;
