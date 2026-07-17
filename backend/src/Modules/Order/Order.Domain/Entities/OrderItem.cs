using Order.Domain.Enums;

namespace Order.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid SellableItemId { get; private set; }
    public OrderItemType SellableItemType { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineSubtotal { get; private set; }

    private OrderItem()
    {
    }

    internal static OrderItem Create(Guid orderId, Guid sellableItemId, OrderItemType sellableItemType, int quantity, decimal unitPrice)
    {
        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            SellableItemId = sellableItemId,
            SellableItemType = sellableItemType,
            Quantity = quantity,
            UnitPrice = unitPrice,
            LineSubtotal = unitPrice * quantity
        };
    }
}
