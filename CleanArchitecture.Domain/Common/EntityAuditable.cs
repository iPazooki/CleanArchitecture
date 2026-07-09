namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Represents a base class for auditable entities, inheriting from <see cref="Entity"/>.
/// </summary>
public abstract class EntityAuditable : Entity
{
    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    /// <remarks>
    /// Written by the persistence layer's auditing interceptor through EF Core's change
    /// tracker, so no caller can rewrite the audit trail.
    /// </remarks>
    public DateTimeOffset CreatedDate { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the date and time when the entity was last updated.
    /// </summary>
    /// <remarks>
    /// Written by the persistence layer's auditing interceptor through EF Core's change
    /// tracker, so no caller can rewrite the audit trail.
    /// </remarks>
    public DateTimeOffset UpdatedDate { get; private set; } = DateTimeOffset.UtcNow;
}
