namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Represents a base class for auditable entities, inheriting from <see cref="Entity"/>.
/// </summary>
public class EntityAuditable : Entity
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.UtcNow;
}
