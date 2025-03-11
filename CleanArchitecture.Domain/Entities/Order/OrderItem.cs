namespace CleanArchitecture.Domain.Entities;

public sealed class OrderItem : Entity
{
    public Guid OrderId { get; init; }

    public Guid BookId { get; init; }

    public required Book Book { get; init; }

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }
}
