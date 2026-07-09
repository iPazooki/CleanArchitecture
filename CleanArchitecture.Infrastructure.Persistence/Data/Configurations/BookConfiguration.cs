using CleanArchitecture.Domain.Validations;

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

        // Named explicitly. EF would otherwise derive it from the IApplicationDbContext.Books
        // DbSet property, silently renaming the existing table.
        builder.ToTable("Book");

        // Keeps the column length in sync with the domain invariant (single source of truth).
        builder.Property(b => b.Title)
            .HasMaxLength(BookRules.TitleMaxLength)
            .IsRequired();

        // Genre is an owned value object. Left unconfigured, its code maps to an unbounded,
        // nullable text column, which contradicts the domain invariant.
        builder.OwnsOne(b => b.Genre, genre => genre
            .Property(g => g.Code)
            .HasMaxLength(BookRules.GenreMaxLength)
            .IsRequired());

        builder.Navigation(b => b.Genre).IsRequired();

        builder.HasIndex(b => b.Title);
    }
}
