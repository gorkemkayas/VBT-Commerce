using Order.Domain.Enums;
using Order.Domain.Exceptions;

namespace Order.Domain.Entities;

/// <summary>
/// Aggregate root for a placed order. Owned by exactly one of a registered user
/// (<see cref="UserId"/>, matching Cart's ShoppingCart.UserId naming) or a guest customer
/// (<see cref="GuestCustomerId"/>). The shipping address is embedded as an immutable snapshot
/// (not a reference) so the order stays historically accurate even if the source address is later
/// edited or deleted. Status is intentionally limited to the order/payment lifecycle
/// (Pending/Confirmed/Cancelled) — delivery progress belongs entirely to Shipping's own
/// Shipment.Status, looked up separately via <see cref="ShipmentId"/>.
/// </summary>
public class CustomerOrder
{
    private readonly List<OrderItem> _items = [];
    private readonly List<OrderCoupon> _coupons = [];

    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? GuestCustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? CancelledReason { get; private set; }

    public string RecipientName { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string District { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;
    public string AddressLine1 { get; private set; } = null!;
    public string? AddressLine2 { get; private set; }

    public Guid ShippingCompanyId { get; private set; }
    public Guid ShipmentId { get; private set; }
    public decimal ShippingFee { get; private set; }

    public decimal Subtotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrandTotal { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<OrderCoupon> Coupons => _coupons.AsReadOnly();

    private CustomerOrder()
    {
    }

    /// <summary>
    /// Takes a caller-supplied <paramref name="id"/> rather than generating its own — the same id
    /// must already have been used as the Inventory reservation ReferenceId and the Shipping
    /// CreateShipmentCommand's OrderId before this aggregate is persisted (see OrderOperations).
    /// </summary>
    public static CustomerOrder Create(
        Guid id,
        Guid? userId,
        Guid? guestCustomerId,
        string recipientName,
        string phoneNumber,
        string country,
        string city,
        string district,
        string postalCode,
        string addressLine1,
        string? addressLine2,
        Guid shippingCompanyId,
        Guid shipmentId,
        decimal shippingFee,
        IReadOnlyCollection<(Guid SellableItemId, OrderItemType SellableItemType, int Quantity, decimal UnitPrice)> items,
        IReadOnlyCollection<(string Code, decimal DiscountAmount)> coupons,
        decimal subtotal,
        decimal discountAmount,
        decimal taxRate,
        decimal taxAmount)
    {
        if (userId is null == guestCustomerId is null)
            throw new ArgumentException("Exactly one of userId or guestCustomerId must be provided.");

        var order = new CustomerOrder
        {
            Id = id,
            UserId = userId,
            GuestCustomerId = guestCustomerId,
            Status = OrderStatus.Pending,
            RecipientName = recipientName,
            PhoneNumber = phoneNumber,
            Country = country,
            City = city,
            District = district,
            PostalCode = postalCode,
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            ShippingCompanyId = shippingCompanyId,
            ShipmentId = shipmentId,
            ShippingFee = shippingFee,
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            TaxRate = taxRate,
            TaxAmount = taxAmount,
            GrandTotal = subtotal - discountAmount + taxAmount + shippingFee,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in items)
            order._items.Add(OrderItem.Create(id, item.SellableItemId, item.SellableItemType, item.Quantity, item.UnitPrice));

        foreach (var coupon in coupons)
            order._coupons.Add(OrderCoupon.Create(id, coupon.Code, coupon.DiscountAmount));

        return order;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new OrderInvalidStatusTransitionException(Id, Status);

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string? reason)
    {
        if (Status != OrderStatus.Pending)
            throw new OrderInvalidStatusTransitionException(Id, Status);

        Status = OrderStatus.Cancelled;
        CancelledReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
}
