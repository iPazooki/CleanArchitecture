namespace CleanArchitecture.Application.Common.Interfaces;

/// <summary>
/// Provides access to the current authenticated user's details.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets the username of the current user, or null if the user is not authenticated.
    /// </summary>
    string? UserName { get; }
}
