using FluentAssertions;
using Moq;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Domain.Entities;

namespace OrderManagement.UnitTests.Orders;

public sealed class GetOrderByIdHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _handler = new GetOrderByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingOrder_ShouldReturnOrderDto()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), [new Order.OrderItemData("Product", 2, 15.00m)]);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var query = new GetOrderByIdQuery(order.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);
        result.TotalAmount.Should().Be(30.00m);
    }

    [Fact]
    public async Task Handle_WithNonExistentOrder_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var act = async () => await _handler.Handle(new GetOrderByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
