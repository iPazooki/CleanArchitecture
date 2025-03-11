namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Represents the base entity class that other entity classes can inherit from.
/// </summary>
public abstract class Entity : IEqualityComparer<Entity>
{
    public Guid Id { get; } = Guid.NewGuid();

    private readonly List<INotification> _domainEvents = [];

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification domainEvent) => _domainEvents.Add(domainEvent);

    public void RemoveDomainEvent(INotification domainEvent) => _domainEvents.Remove(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public bool Equals(Entity? x, Entity? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        return x.Id.Equals(y.Id) && x._domainEvents.Equals(y._domainEvents);
    }

    public int GetHashCode(Entity obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return HashCode.Combine(obj.Id, obj._domainEvents);
    }
}
