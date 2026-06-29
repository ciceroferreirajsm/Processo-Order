using FluentAssertions;
using Moq;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Orders.Commands.CancelOrder;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.UnitTests.Orders;

public sealed class CancelOrderHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly CancelOrderCommandHandler _handler;

    public CancelOrderHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _handler = new CancelOrderCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithPendingOrder_ShouldCancelSuccessfully()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), [new Order.OrderItemData("Product", 1, 10.00m)]);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CancelOrderCommand(order.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var command = new CancelOrderCommand(Guid.NewGuid());

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_WithAlreadyCancelledOrder_ShouldThrowDomainException()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), [new Order.OrderItemData("Product", 1, 10.00m)]);
        order.Cancel();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var command = new CancelOrderCommand(order.Id);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
