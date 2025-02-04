namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// Represents a book entity, inheriting from <see cref="BaseAuditableEntity"/>.
/// </summary>
public sealed class Book : BaseAuditableEntity
{
    /// <summary>
    /// Gets or sets the title of the book.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the genre of the book.
    /// </summary>
    public required Genre Genre { get; set; }
}