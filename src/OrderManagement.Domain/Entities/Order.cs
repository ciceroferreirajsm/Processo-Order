using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Entities;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // Computed in domain, not in application layer
    public decimal TotalAmount => _items.Sum(i => i.GetSubtotal());

    private Order() { }

    public record OrderItemData(string ProductName, int Quantity, decimal UnitPrice);

    public static Order Create(Guid customerId, IReadOnlyList<OrderItemData> itemData)
    {
        if (itemData is null || itemData.Count == 0)
            throw new DomainException("An order must have at least one item.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var data in itemData)
            order._items.Add(new OrderItem(order.Id, data.ProductName, data.Quantity, data.UnitPrice));

        return order;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Only pending orders can be cancelled. Current status: {Status}.");

        Status = OrderStatus.Cancelled;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Only pending orders can be confirmed. Current status: {Status}.");

        Status = OrderStatus.Confirmed;
    }
}
