namespace CleanArchitecture.Application.Entities.Login.Commands;

/// <summary>
/// Command for logging in a user.
/// </summary>
/// <param name="Email">The email of the user.</param>
/// <param name="Password">The password of the user</param>
/// <returns>A result containing a string, which could be a token or a message.</returns>
public record LoginCommand(string Email, string Password) : IRequest<Result<string>>;