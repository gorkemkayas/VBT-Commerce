using Cart.Contracts;
using Cart.Domain.Enums;
using Inventory.Application.Commands.Reservations.ConfirmReservationsByReference;
using Inventory.Application.Commands.Reservations.ReleaseReservationsByReference;
using Inventory.Application.Commands.Reservations.ReserveStock;
using Inventory.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;
using Order.Application.Common;
using Order.Application.Integrations;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Pricing.Application.Commands.CouponUsage.CommitCouponUsage;
using Pricing.Application.Common;
using Pricing.Domain.Enums;
using Shipping.Application.Commands.Shipments.CreateShipment;

namespace Order.Application.Services;

/// <summary>
/// Shared checkout/confirm/cancel logic used by both the self-service ("me") and guest order
/// handlers — mirrors Cart's CartOperations and Pricing's PriceCalculationService. Orchestrates
/// Inventory/Shipping/Pricing/Cart via the shared MediatR ISender (see the plan's "cross-module
/// orchestration" note): this is a saga, not a single distributed transaction — each step below that
/// has already produced a side effect is explicitly compensated if a later step fails.
/// </summary>
public class OrderOperations(
    IOrderDbContext dbContext,
    ISender sender,
    IShippingIntegrationService shippingIntegrationService,
    ILogger<OrderOperations> logger)
{
    public async Task<Guid> PlaceOrderAsync(
        OrderOwnerKey owner,
        IReadOnlyCollection<CartItemSummaryDto> cartItems,
        OrderAddressSnapshot address,
        Guid shippingCompanyId,
        PriceCalculationResultDto priceResult,
        Func<CancellationToken, Task> clearCartAsync,
        CancellationToken cancellationToken)
    {
        var shippingCompany = await shippingIntegrationService.GetActiveShippingCompanyAsync(shippingCompanyId, cancellationToken)
            ?? throw new OrderShippingCompanyUnavailableException(shippingCompanyId);

        var orderId = Guid.NewGuid();

        var reserveItems = cartItems
            .Select(i => new ReserveStockLineItem(i.SellableItemId, MapToInventoryItemType(i.SellableItemType), i.Quantity))
            .ToList();

        await sender.Send(new ReserveStockCommand(orderId, reserveItems), cancellationToken);

        Guid shipmentId;
        try
        {
            shipmentId = await sender.Send(new CreateShipmentCommand(orderId, shippingCompanyId), cancellationToken);
        }
        catch
        {
            await sender.Send(new ReleaseReservationsByReferenceCommand(orderId), CancellationToken.None);
            throw;
        }

        var orderItems = priceResult.Lines
            .Select(l => (l.SellableItemId, MapToOrderItemType(l.SellableItemType), l.Quantity, l.UnitPrice))
            .ToList();
        var orderCoupons = priceResult.AppliedCoupons
            .Select(c => (c.Code, c.DiscountAmount))
            .ToList();

        var order = CustomerOrder.Create(
            orderId,
            owner.UserId,
            owner.GuestCustomerId,
            address.RecipientName,
            address.PhoneNumber,
            address.Country,
            address.City,
            address.District,
            address.PostalCode,
            address.AddressLine1,
            address.AddressLine2,
            shippingCompanyId,
            shipmentId,
            shippingCompany.Fee,
            orderItems,
            orderCoupons,
            priceResult.Subtotal,
            priceResult.TotalDiscount,
            priceResult.TaxRate,
            priceResult.TaxAmount);

        try
        {
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await sender.Send(new ReleaseReservationsByReferenceCommand(orderId), CancellationToken.None);
            throw;
        }

        try
        {
            await clearCartAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Failed to clear the cart after placing order '{OrderId}'.", orderId);
        }

        return orderId;
    }

    public async Task ConfirmAsync(CustomerOrder order, CancellationToken cancellationToken)
    {
        if (order.Status != OrderStatus.Pending)
            throw new OrderInvalidStatusTransitionException(order.Id, order.Status);

        await sender.Send(new ConfirmReservationsByReferenceCommand(order.Id), cancellationToken);

        if (order.Coupons.Count > 0)
        {
            var appliedCoupons = order.Coupons.Select(c => new AppliedCouponDto(c.Code, c.DiscountAmount)).ToList();
            await sender.Send(new CommitCouponUsageCommand(appliedCoupons, order.UserId, order.GuestCustomerId, order.Id), cancellationToken);
        }

        order.Confirm();
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelAsync(CustomerOrder order, string? reason, CancellationToken cancellationToken)
    {
        if (order.Status != OrderStatus.Pending)
            throw new OrderInvalidStatusTransitionException(order.Id, order.Status);

        await sender.Send(new ReleaseReservationsByReferenceCommand(order.Id), cancellationToken);

        order.Cancel(reason);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static InventoryItemType MapToInventoryItemType(CartItemType type) => type switch
    {
        CartItemType.Product => InventoryItemType.Product,
        CartItemType.Variant => InventoryItemType.Variant,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    private static OrderItemType MapToOrderItemType(PriceItemType type) => type switch
    {
        PriceItemType.Product => OrderItemType.Product,
        PriceItemType.Variant => OrderItemType.Variant,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
