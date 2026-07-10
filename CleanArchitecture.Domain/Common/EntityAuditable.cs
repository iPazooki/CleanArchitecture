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

    /// <summary>
    /// Gets the username of the user who last updated the entity.
    /// </summary>
    /// <remarks>
    /// Written by the persistence layer's auditing interceptor through EF Core's change
    /// tracker, so no caller can rewrite the audit trail.
    /// </remarks>
#pragma warning disable S1144 // EF Core writes to this property via the change tracker/reflection
    public string? UpdatedBy { get; private set; }
#pragma warning restore S1144
}
