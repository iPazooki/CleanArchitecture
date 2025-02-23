using User = CleanArchitecture.Domain.Entities.User.User;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

/// <summary>
/// Configures the User entity.
/// </summary>
public class UserConfiguration : BaseAggregateRootConfiguration<User>
{
    /// <summary>
    /// Configures the properties and relationships of the User entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the User entity.</param>
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
        
        builder.Property(p=> p.Email)
            .HasMaxLength(100)
            .IsRequired(false);
        
        builder.Property(p => p.HashedPassword)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.HasIndex(p => p.LastName);
    }
}