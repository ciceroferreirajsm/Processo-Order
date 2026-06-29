using FluentAssertions;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.UnitTests.Domain;

public sealed class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnOrderWithPendingStatus()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var items = new List<Order.OrderItemData>
        {
            new("Product A", 2, 10.00m)
        };

        // Act
        var order = Order.Create(customerId, items);

        // Assert
        order.Id.Should().NotBe(Guid.Empty);
        order.CustomerId.Should().Be(customerId);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(20.00m);
    }

    [Fact]
    public void Create_WithNoItems_ShouldThrowDomainException()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var act = () => Order.Create(customerId, []);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*at least one item*");
    }

    [Fact]
    public void Cancel_WhenPending_ShouldSetStatusToCancelled()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), [new("Product", 1, 5.00m)]);

        // Act
        order.Cancel();

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldThrowDomainException()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), [new("Product", 1, 5.00m)]);
        order.Cancel();

        // Act
        var act = () => order.Cancel();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*pending*");
    }

    [Fact]
    public void TotalAmount_ShouldBeCalculatedInDomain()
    {
        // Arrange
        var items = new List<Order.OrderItemData>
        {
            new("Product A", 3, 10.00m),
            new("Product B", 2, 5.50m)
        };

        // Act
        var order = Order.Create(Guid.NewGuid(), items);

        // Assert
        order.TotalAmount.Should().Be(41.00m); // (3*10) + (2*5.50)
    }
}
