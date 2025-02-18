using CleanArchitecture.Domain.Entities.Person;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

/// <summary>
/// Configures the Person entity.
/// </summary>
public class PersonConfiguration : BaseAggregateRootConfiguration<Person>
{
    /// <summary>
    /// Configures the properties and relationships of the Person entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the Person entity.</param>
    public override void Configure(EntityTypeBuilder<Person> builder)
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

        builder.HasIndex(p => p.LastName);
    }
}