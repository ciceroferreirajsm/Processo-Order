using MediatR;
using OrderManagement.Application.Orders.DTOs;

namespace OrderManagement.Application.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;
