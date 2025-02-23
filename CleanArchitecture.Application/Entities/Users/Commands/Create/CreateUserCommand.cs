namespace CleanArchitecture.Application.Entities.Users.Commands.Create;

/// <summary>
/// Command for creating a user.
/// </summary>
/// <param name="FirstName">The first name of the user.</param>
/// <param name="LastName">The last name of the user.</param>
/// <param name="Email">The email address of the user (optional).</param>
/// <param name="Password">The password for the user (optional).</param>
/// <param name="Address">The address of the user (optional).</param>
/// <param name="Gender">The gender of the user (optional).</param>
/// <returns>A result containing a GUID representing the user ID.</returns>
public record CreateUserCommand(string FirstName, string LastName, string? Email, string? Password, AddressRequest? Address, int? Gender) : IRequest<Result<Guid>>;