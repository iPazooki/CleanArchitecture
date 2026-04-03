namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Represents the base entity class that other entity classes can inherit from.
/// Identity is determined solely by <see cref="Id"/>; domain events are transient and excluded from equality.
/// </summary>
#pragma warning disable S4035 // IEquatable on abstract base entity is intentional for DDD identity-based equality
public abstract class Entity : IEquatable<Entity>
#pragma warning restore S4035
{
    public Guid Id { get; } = Guid.NewGuid();

    private readonly List<INotification> _domainEvents = [];

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification domainEvent) => _domainEvents.Add(domainEvent);

    public void RemoveDomainEvent(INotification domainEvent) => _domainEvents.Remove(domainEvent);

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