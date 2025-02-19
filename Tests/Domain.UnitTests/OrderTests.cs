using CleanArchitecture.Domain.Events.Order;
using CleanArchitecture.Domain.Entities.Book;
using CleanArchitecture.Domain.Entities.Order;
using CleanArchitecture.Domain.Entities.Person;

namespace Domain.UnitTests;

public class OrderTests
{
    [Fact]
    public void Order_Create_ShouldReturnSuccessResult_WhenStatusIsPending()
    {
        // Arrange & Act
        Result<Order> result = Order.Create(Person.Create("First Name", "Last Name")!, OrderStatus.Pending);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(OrderStatus.Pending, result.Value.OrderStatus);
        Assert.Single(result.Value.DomainEvents);
        Assert.IsType<OrderAddedEvent>(result.Value.DomainEvents.First());
    }

    [Fact]
    public void Order_Create_ShouldReturnFailure_WhenStatusIsNotPending()
    {
        // Arrange & Act
        Result<Order> result = Order.Create(Person.Create("First Name", "Last Name")!, OrderStatus.Delivered);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(OrderErrors.OrderStatusInvalid, result.Errors);
    }

    [Fact]
    public void Order_AddOrderItem_ShouldSucceed_WhenValidData()
    {
        // Arrange
        Order order = Order.Create(Person.Create("First Name", "Last Name")!, OrderStatus.Pending).Value!;

        // Act
        Result addResult = order.AddOrderItem(Book.Create("Some Book", Genre.Fiction)!, 2, 10m);

        // Assert
        Assert.True(addResult.IsSuccess);
        Assert.Single(order.OrderItems);
        Assert.Equal(2, order.OrderItems.First().Quantity);
        Assert.Equal(10m, order.OrderItems.First().UnitPrice);
    }

    [Fact]
    public void Order_AddOrderItem_ShouldFail_WhenQuantityIsInvalid()
    {
        // Arrange
        Order order = Order.Create(Person.Create("First Name", "Last Name")!, OrderStatus.Pending).Value!;

        // Act
        Result addResult = order.AddOrderItem(Book.Create("Invalid Qty", Genre.Fiction)!, 0, 10m);

        // Assert
        Assert.False(addResult.IsSuccess);
        Assert.Contains(OrderErrors.OrderItemQuantityInvalid, addResult.Errors);
    }

    [Fact]
    public void Order_RemoveOrderItem_ShouldRemoveCorrectly()
    {
        // Arrange
        Order order = Order.Create(Person.Create("First Name", "Last Name")!, OrderStatus.Pending).Value!;
        order.AddOrderItem(Book.Create("Test Book", Genre.Fiction)!, 1, 15m);
        OrderItem toRemove = order.OrderItems.First();

        // Act
        order.RemoveOrderItem(toRemove);

        // Assert
        Assert.Empty(order.OrderItems);
    }
}