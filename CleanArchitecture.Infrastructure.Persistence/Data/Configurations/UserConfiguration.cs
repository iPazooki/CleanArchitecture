using CleanArchitecture.Domain.Entities.Security;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

internal sealed class UserConfiguration : BaseAggregateRootConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(100).IsRequired();
            address.Property(a => a.City).HasMaxLength(50).IsRequired();
            address.Property(a => a.PostalCode).HasMaxLength(10).IsRequired();
        });

        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .IsRequired(false);

        builder.Property(p => p.Email)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(p => p.HashedPassword)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.HasMany(p => p.Roles)
            .WithMany()
            .UsingEntity<RoleUser>(r =>
                    r.HasOne<Role>().WithMany().HasForeignKey(ru => ru.RoleId),
                u =>
                    u.HasOne<User>().WithMany().HasForeignKey(ru => ru.UserId));

        builder.HasIndex(p => p.Email).IsUnique();
    }
}
