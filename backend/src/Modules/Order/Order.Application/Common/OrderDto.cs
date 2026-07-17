using Order.Domain.Enums;

namespace Order.Application.Common;

public record OrderItemDto(Guid SellableItemId, OrderItemType SellableItemType, int Quantity, decimal UnitPrice, decimal LineSubtotal);

public record OrderCouponDto(string Code, decimal DiscountAmount);

public record OrderDto(
    Guid Id,
    Guid? UserId,
    Guid? GuestCustomerId,
    OrderStatus Status,
    string? CancelledReason,
    string RecipientName,
    string PhoneNumber,
    string Country,
    string City,
    string District,
    string PostalCode,
    string AddressLine1,
    string? AddressLine2,
    Guid ShippingCompanyId,
    Guid ShipmentId,
    decimal ShippingFee,
    IReadOnlyCollection<OrderItemDto> Items,
    IReadOnlyCollection<OrderCouponDto> Coupons,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TaxRate,
    decimal TaxAmount,
    decimal GrandTotal,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
