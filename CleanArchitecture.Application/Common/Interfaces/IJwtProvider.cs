using CleanArchitecture.Domain.Entities.User;

namespace CleanArchitecture.Application.Common.Interfaces;

/// <summary>
/// Interface for JWT provider service.
/// </summary>
public interface IJwtProvider
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the JWT token is generated.</param>
    /// <returns>A JWT token as a string.</returns>
    string GenerateJwtToken(User user);
}