namespace Order.Application.Services;

/// <summary>
/// Identifies who an order belongs to — exactly one of a registered user or a guest customer.
/// Mirrors Cart's CartOwnerKey.
/// </summary>
public class OrderOwnerKey
{
    public Guid? UserId { get; }
    public Guid? GuestCustomerId { get; }

    private OrderOwnerKey(Guid? userId, Guid? guestCustomerId)
    {
        UserId = userId;
        GuestCustomerId = guestCustomerId;
    }

    public static OrderOwnerKey ForUser(Guid userId) => new(userId, null);

    public static OrderOwnerKey ForGuestCustomer(Guid guestCustomerId) => new(null, guestCustomerId);
}
