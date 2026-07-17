using Order.Domain.Entities;

namespace Order.Application.Common;

public static class OrderMapper
{
    public static OrderDto ToDto(CustomerOrder order)
        => new(
            order.Id,
            order.UserId,
            order.GuestCustomerId,
            order.Status,
            order.CancelledReason,
            order.RecipientName,
            order.PhoneNumber,
            order.Country,
            order.City,
            order.District,
            order.PostalCode,
            order.AddressLine1,
            order.AddressLine2,
            order.ShippingCompanyId,
            order.ShipmentId,
            order.ShippingFee,
            order.Items.Select(i => new OrderItemDto(i.SellableItemId, i.SellableItemType, i.Quantity, i.UnitPrice, i.LineSubtotal)).ToList(),
            order.Coupons.Select(c => new OrderCouponDto(c.Code, c.DiscountAmount)).ToList(),
            order.Subtotal,
            order.DiscountAmount,
            order.TaxRate,
            order.TaxAmount,
            order.GrandTotal,
            order.CreatedAt,
            order.UpdatedAt);
}
