namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// Represents a book entity, inheriting from <see cref="EntityAuditable"/>.
/// </summary>
public sealed partial class Book : AggregateRootAuditable
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
