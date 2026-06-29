using OrderManagement.Application.Orders.DTOs;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Mappings;

internal static class OrderMappings
{
    public static OrderDto ToDto(this Order order) => new(
        order.Id,
        order.CustomerId,
        order.Status.ToString(),
        order.CreatedAt,
        order.TotalAmount,
        order.Items.Select(i => i.ToDto()).ToList()
    );

    public static OrderItemDto ToDto(this OrderItem item) => new(
        item.Id,
        item.ProductName,
        item.Quantity,
        item.UnitPrice,
        item.GetSubtotal()
    );
}
