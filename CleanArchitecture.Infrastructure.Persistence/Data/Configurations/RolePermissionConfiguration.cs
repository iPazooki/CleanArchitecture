using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Entities.Security;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasKey(x => new { x.RoleId, x.PermissionId });

        builder.HasData(
            Create(Roles.Administrator, Permissions.CanRead),
            Create(Roles.Administrator, Permissions.CanWrite),
            Create(Roles.Administrator, Permissions.CanDelete),
            Create(Roles.Administrator, Permissions.CanUpdate),
            Create(Roles.Administrator, Permissions.CanReject),
            Create(Roles.Administrator, Permissions.CanApprove),
            Create(Roles.Owner, Permissions.CanRead),
            Create(Roles.Owner, Permissions.CanWrite),
            Create(Roles.Member, Permissions.CanRead)
        );
    }

    private static RolePermission Create(Roles roles, Permissions permissions) =>
        new()
        {
            RoleId = (int)roles,
            PermissionId = (int)permissions
        };
}
