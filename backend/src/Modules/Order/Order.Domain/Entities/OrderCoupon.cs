namespace Order.Domain.Entities;

public class OrderCoupon
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string Code { get; private set; } = null!;
    public decimal DiscountAmount { get; private set; }

    private OrderCoupon()
    {
    }

    internal static OrderCoupon Create(Guid orderId, string code, decimal discountAmount)
    {
        return new OrderCoupon
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Code = code,
            DiscountAmount = discountAmount
        };
    }
}
