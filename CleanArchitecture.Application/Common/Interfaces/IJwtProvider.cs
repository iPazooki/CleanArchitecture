using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces;

/// <summary>
/// Interface for generating JWT tokens.
/// </summary>
public interface IJwtProvider
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the generated JWT token as a string.</returns>
    Task<string> GenerateJwtTokenAsync(User user);
}
