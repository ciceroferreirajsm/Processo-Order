using MediatR;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Orders.DTOs;
using OrderManagement.Application.Orders.Mappings;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var itemData = request.Items
            .Select(i => new Order.OrderItemData(i.ProductName, i.Quantity, i.UnitPrice))
            .ToList();

        var order = Order.Create(request.CustomerId, itemData);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return order.ToDto();
    }
}
