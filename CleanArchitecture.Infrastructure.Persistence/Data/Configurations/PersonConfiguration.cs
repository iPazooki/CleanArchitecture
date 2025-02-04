namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

/// <summary>
/// Configures the Person entity.
/// </summary>
public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    /// <summary>
    /// Configures the properties and relationships of the Person entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the Person entity.</param>
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        // Configures the FirstName property with a maximum length of 50 and makes it required.
        builder.Property(p => p.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        // Configures the LastName property with a maximum length of 50 and makes it required.
        builder.Property(p => p.LastName)
            .HasMaxLength(50)
            .IsRequired();

        // Configures the Address property as an owned entity.
        builder.OwnsOne(b => b.Address);

        // Configures the Gender property with a string conversion and makes it optional.
        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .IsRequired(false);
    }
}