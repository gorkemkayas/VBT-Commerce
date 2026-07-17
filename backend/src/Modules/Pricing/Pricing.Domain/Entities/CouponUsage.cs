namespace Pricing.Domain.Entities;

/// <summary>
/// Records a single application of a coupon to an order, for total/per-user usage-limit enforcement.
/// Exactly one of <see cref="CustomerId"/> (a registered user's UserId) or <see cref="GuestCustomerId"/>
/// (a Customer-module GuestCustomer record) is set — mirrors Cart's ownership-key pattern.
/// </summary>
public class CouponUsage
{
    public Guid Id { get; private set; }
    public Guid CouponId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid? GuestCustomerId { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public DateTime UsedAt { get; private set; }

    private CouponUsage()
    {
    }

    public static CouponUsage Create(Guid couponId, Guid? customerId, Guid? guestCustomerId, Guid orderId, decimal discountAmount)
    {
        if (customerId is null == guestCustomerId is null)
            throw new ArgumentException("Exactly one of customerId or guestCustomerId must be provided.");

        return new CouponUsage
        {
            Id = Guid.NewGuid(),
            CouponId = couponId,
            CustomerId = customerId,
            GuestCustomerId = guestCustomerId,
            OrderId = orderId,
            DiscountAmount = discountAmount,
            UsedAt = DateTime.UtcNow
        };
    }
}
