namespace CleanArchitecture.Domain.Entities.Security;

public sealed class RoleUser
{
    public required Guid UserId { get; init; }

    public required int RoleId { get; init; }
}