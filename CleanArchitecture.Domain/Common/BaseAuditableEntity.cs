namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Represents a base class for auditable entities, inheriting from <see cref="BaseEntity"/>.
/// </summary>
public class BaseAuditableEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    public DateTimeOffset UpdatedDate { get; set; } = DateTime.UtcNow;
}