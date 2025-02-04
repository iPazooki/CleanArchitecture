﻿namespace CleanArchitecture.Infrastructure.Persistence.Data.Configurations;

/// <summary>
/// Configures the Book entity.
/// </summary>
public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    /// <summary>
    /// Configures the properties and relationships of the Book entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the Book entity.</param>
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        // Configures the Title property with a maximum length of 200 and makes it required.
        builder.Property(b => b.Title)
            .HasMaxLength(200)
            .IsRequired();

        // Configures the Genre property as an owned entity.
        builder.OwnsOne(b => b.Genre);
    }
}