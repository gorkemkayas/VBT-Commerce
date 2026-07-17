namespace Pricing.Application.Services;

/// <summary>
/// Identifies who a price calculation/coupon usage is for — exactly one of a registered user
/// (CustomerId = UserId) or a guest checkout's GuestCustomerId. Mirrors Cart's CartOwnerKey.
/// </summary>
public class PricingOwnerKey
{
    public Guid? CustomerId { get; }
    public Guid? GuestCustomerId { get; }

    private PricingOwnerKey(Guid? customerId, Guid? guestCustomerId)
    {
        CustomerId = customerId;
        GuestCustomerId = guestCustomerId;
    }

    public static PricingOwnerKey ForCustomer(Guid customerId) => new(customerId, null);

    public static PricingOwnerKey ForGuestCustomer(Guid guestCustomerId) => new(null, guestCustomerId);
}
