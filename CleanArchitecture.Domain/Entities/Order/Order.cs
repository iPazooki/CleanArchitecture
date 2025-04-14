using CleanArchitecture.Domain.Events.Order;

namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// Represents an order in the domain.
/// Inherits from <see cref="AggregateRoot"/>.
/// </summary>
public sealed partial class Order : AggregateRoot
{
    /// <summary>
    /// Gets the customer associated with the order.
    /// </summary>
    public required User Customer { get; init; }

    /// <summary>
    /// Gets the ID of the customer associated with the order.
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    /// Gets or sets the status of the order.
    /// When the status changes, an <see cref="OrderStatusChangedEvent"/> is added to the domain events.
    /// </summary>
    public OrderStatus OrderStatus
    {
        get => _orderStatus;
        set
        {
            if (value != _orderStatus)
            {
                AddDomainEvent(new OrderStatusChangedEvent(this));
            }

            _orderStatus = value;
        }
    }

    /// <summary>
    /// Gets the date and time when the order was purchased.
    /// </summary>
    public DateTimeOffset PurchasedDateTime { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the collection of items in the order.
    /// </summary>
    public ICollection<OrderItem> OrderItems => _orderItems;
}
