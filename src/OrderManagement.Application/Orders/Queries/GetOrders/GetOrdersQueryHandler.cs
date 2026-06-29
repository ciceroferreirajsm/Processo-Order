using MediatR;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Orders.DTOs;
using OrderManagement.Application.Orders.Mappings;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _orderRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);

        return new PagedResult<OrderDto>(
            items.Select(o => o.ToDto()).ToList(),
            totalCount,
            request.Page,
            request.PageSize
        );
    }
}
