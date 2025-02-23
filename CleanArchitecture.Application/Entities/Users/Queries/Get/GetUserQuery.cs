namespace CleanArchitecture.Application.Entities.Users.Queries.Get;

/// <summary>
/// Query for getting a user by ID.
/// </summary>
/// <param name="Id">The ID of the user.</param>
/// <returns>A result containing the user response.</returns>
public record GetUserQuery(Guid Id) : IRequest<Result<UserResponse>>;