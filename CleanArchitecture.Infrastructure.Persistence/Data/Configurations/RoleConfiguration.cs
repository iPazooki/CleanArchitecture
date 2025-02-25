using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Entities.Security;
using CleanArchitecture.Domain.Entities.User;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.Permissions)
            .WithMany()
            .UsingEntity<RolePermission>();

        builder.HasMany(x => x.Users)
            .WithMany()
            .UsingEntity<RoleUser>(u => 
                    u.HasOne<User>().WithMany().HasForeignKey(ru => ru.UserId),
                r => 
                    r.HasOne<Role>().WithMany().HasForeignKey(ru => ru.RoleId));

        IEnumerable<Role> roles = Enum.GetValues<Roles>().Select(x => new Role { Id = (int)x, Name = x.ToString() });

        builder.HasData(roles);
    }
}