using CleanArchitecture.Domain.Validations.Book;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

/// <summary>
/// Configures the Book entity.
/// </summary>
internal sealed class BookConfiguration : BaseAggregateRootAuditableConfiguration<Book>
{
    /// <summary>
    /// Configures the properties and relationships of the Book entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the Book entity.</param>
    public override void Configure(EntityTypeBuilder<Book> builder)
    {
        base.Configure(builder);

        // Keeps the column length in sync with the domain invariant (single source of truth).
        builder.Property(b => b.Title)
            .HasMaxLength(BookRules.TitleMaxLength)
            .IsRequired();

        // Configures the Genre property as an owned entity.
        builder.OwnsOne(b => b.Genre);

        builder.HasIndex(b => b.Title);
    }
}
