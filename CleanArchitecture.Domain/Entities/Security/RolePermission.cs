namespace CleanArchitecture.Domain.Entities.Security;

public sealed class RolePermission
{
    public required int RoleId { get; init; }

    public required int PermissionId { get; init; }
}