using MediatR;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} not found.");

        order.Cancel(); // Domain enforces business rule

        await _orderRepository.SaveChangesAsync(cancellationToken);
    }
}
