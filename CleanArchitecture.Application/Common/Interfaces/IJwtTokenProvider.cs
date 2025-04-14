using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces;

/// <summary>
/// Interface for generating JWT tokens.
/// </summary>
public interface IJwtTokenProvider
{

    Task<(string Token, DateTimeOffset ExpirationDate)> GenerateJwtTokenAsync(User user);


    (string Token, DateTimeOffset ExpirationDate) GenerateRefreshToken();
}
