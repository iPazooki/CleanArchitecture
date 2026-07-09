namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Represents the base entity class that other entity classes can inherit from.
/// Identity is determined solely by <see cref="Id"/>; domain events are transient and excluded from equality.
/// </summary>
#pragma warning disable S4035 // IEquatable on abstract base entity is intentional for DDD identity-based equality
public abstract class Entity : IEquatable<Entity>
#pragma warning restore S4035
{
    // Version 7 GUIDs are time-ordered, so key inserts append to the index rather than
    // scattering across it the way random version 4 GUIDs do.
    public Guid Id { get; } = Guid.CreateVersion7();

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Records a domain event. Only the aggregate itself may raise events about its own state.
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Drops the recorded events once they have been dispatched.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    public bool Equals(Entity? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as Entity);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);

    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
}