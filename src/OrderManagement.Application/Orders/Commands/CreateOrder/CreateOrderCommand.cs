using MediatR;
using OrderManagement.Application.Orders.DTOs;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderItemRequest(
    string ProductName,
    int Quantity,
    decimal UnitPrice
);

public sealed record CreateOrderCommand(
    Guid CustomerId,
    IReadOnlyList<CreateOrderItemRequest> Items
) : IRequest<OrderDto>;
