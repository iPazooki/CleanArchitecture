namespace CleanArchitecture.Application.Common.Interfaces;

/// <summary>
/// Interface for password hashing and verification.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes the provided password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password as a string.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies the provided password against the hashed password.
    /// </summary>
    /// <param name="password">The plain text password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>True if the password matches the hashed password, otherwise false.</returns>
    bool VerifyPassword(string password, string? hashedPassword);
}