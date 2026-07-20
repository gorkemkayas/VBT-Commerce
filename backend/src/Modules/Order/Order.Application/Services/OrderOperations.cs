using System.Text.Json;
using Cart.Contracts;
using Cart.Domain.Enums;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;
using Order.Application.Common;
using Order.Application.Integrations;
using Order.Contracts.Events;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Payment.Contracts;
using Pricing.Contracts;
using Pricing.Domain.Enums;

namespace Order.Application.Services;

/// <summary>
/// Shared checkout/confirm/cancel logic used by both the self-service ("me") and guest order
/// handlers — mirrors Cart's CartOperations and Pricing's PriceCalculationService. Orchestrates
/// Inventory/Shipping/Payment/Pricing/Cart purely through each module's own Contracts-backed
/// integration service (never their Application layers): this is a saga, not a single distributed transaction — each
/// step below that has already produced a side effect is explicitly compensated if a later step
/// fails. Checkout is fully synchronous end-to-end: a declined card means the order is never
/// persisted at all (no Pending row left behind), and a successful charge is immediately followed,
/// in the same call, by the same reservation-confirm/coupon-commit logic used by ConfirmAsync — so
/// the order the caller sees back is always already Confirmed.
/// </summary>
public class OrderOperations(
    IOrderDbContext dbContext,
    IShippingIntegrationService shippingIntegrationService,
    IInventoryIntegrationService inventoryIntegrationService,
    IPricingIntegrationService pricingIntegrationService,
    IPaymentIntegrationService paymentIntegrationService,
    ILogger<OrderOperations> logger)
{
    public async Task<Guid> PlaceOrderAsync(
        OrderOwnerKey owner,
        IReadOnlyCollection<CartItemSummaryDto> cartItems,
        OrderAddressSnapshot address,
        Guid shippingCompanyId,
        PriceCalculationResultDto priceResult,
        PaymentCardInfo card,
        PaymentBuyerInfo buyer,
        Func<CancellationToken, Task> clearCartAsync,
        CancellationToken cancellationToken)
    {
        var shippingCompany = await shippingIntegrationService.GetActiveShippingCompanyAsync(shippingCompanyId, cancellationToken)
            ?? throw new OrderShippingCompanyUnavailableException(shippingCompanyId);

        var orderId = Guid.NewGuid();

        var reserveItems = cartItems
            .Select(i => new ReserveStockLineItem(i.SellableItemId, MapToOrderItemType(i.SellableItemType), i.Quantity))
            .ToList();

        await inventoryIntegrationService.ReserveStockAsync(orderId, reserveItems, cancellationToken);

        Guid shipmentId;
        try
        {
            shipmentId = await shippingIntegrationService.CreateShipmentAsync(orderId, shippingCompanyId, cancellationToken);
        }
        catch
        {
            await inventoryIntegrationService.ReleaseReservationsAsync(orderId, CancellationToken.None);
            throw;
        }

        var orderItems = priceResult.Lines
            .Select(l => (l.SellableItemId, MapToOrderItemType(l.SellableItemType), l.Quantity, l.UnitPrice))
            .ToList();
        var orderCoupons = priceResult.AppliedCoupons
            .Select(c => (c.Code, c.DiscountAmount))
            .ToList();

        var grandTotal = priceResult.Subtotal - priceResult.TotalDiscount + priceResult.TaxAmount + shippingCompany.Fee;

        var basketItems = orderItems
            .Select(i => new PaymentBasketItem($"{i.Item2} {i.SellableItemId}", "General", i.UnitPrice * i.Quantity))
            .Append(new PaymentBasketItem("Shipping", "Shipping", shippingCompany.Fee))
            .ToList();

        var addressInfo = new PaymentAddressInfo(
            $"{address.AddressLine1} {address.AddressLine2}".Trim(), address.City, address.Country, address.PostalCode);

        Guid paymentId;
        try
        {
            paymentId = await paymentIntegrationService.ChargeAsync(
                orderId, priceResult.Subtotal + shippingCompany.Fee, grandTotal, card, buyer, addressInfo, basketItems, cancellationToken);
        }
        catch
        {
            await inventoryIntegrationService.ReleaseReservationsAsync(orderId, CancellationToken.None);
            throw;
        }

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
            paymentId,
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
            await TryRefundAsync(orderId, buyer.Ip, "order persistence failed after a successful charge");
            await inventoryIntegrationService.ReleaseReservationsAsync(orderId, CancellationToken.None);
            throw;
        }

        try
        {
            await ConfirmAsync(order, cancellationToken);
        }
        catch (Exception exception)
        {
            await TryRefundAsync(orderId, buyer.Ip, "reservation confirm/coupon commit failed after a successful charge");
            logger.LogError(exception, "Failed to confirm order '{OrderId}' after a successful payment charge; the order remains Pending.", orderId);
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

        await inventoryIntegrationService.ConfirmReservationsAsync(order.Id, cancellationToken);

        if (order.Coupons.Count > 0)
        {
            var appliedCoupons = order.Coupons.Select(c => new AppliedCouponDto(c.Code, c.DiscountAmount)).ToList();
            await pricingIntegrationService.CommitCouponUsageAsync(appliedCoupons, order.UserId, order.GuestCustomerId, order.Id, cancellationToken);
        }

        order.Confirm();

        var confirmedEvent = new OrderConfirmedEvent(order.Id, order.UserId, order.GuestCustomerId);
        dbContext.OutboxMessages.Add(OutboxMessage.Create(OrderConfirmedEvent.EventType, JsonSerializer.Serialize(confirmedEvent)));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Cancelling a Pending order releases its (not-yet-confirmed) stock reservation. Cancelling a
    /// Confirmed order instead refunds the payment — its reservation cannot be released at that
    /// point because Inventory permanently decrements on-hand quantity when a reservation is
    /// confirmed (StockItem.ReleaseReservation rejects an already-confirmed reservation), so
    /// restocking after a Confirmed cancellation is a deliberate, documented gap (would require a
    /// separate Inventory "restock" feature, out of scope here).
    /// </summary>
    public async Task CancelAsync(CustomerOrder order, string? reason, string ip, CancellationToken cancellationToken)
    {
        switch (order.Status)
        {
            case OrderStatus.Pending:
                await inventoryIntegrationService.ReleaseReservationsAsync(order.Id, cancellationToken);
                break;
            case OrderStatus.Confirmed:
                await paymentIntegrationService.RefundAsync(order.Id, ip, cancellationToken);
                break;
            default:
                throw new OrderInvalidStatusTransitionException(order.Id, order.Status);
        }

        order.Cancel(reason);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task TryRefundAsync(Guid orderId, string ip, string context)
    {
        try
        {
            await paymentIntegrationService.RefundAsync(orderId, ip, CancellationToken.None);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to refund payment for order '{OrderId}' after {Context}.", orderId, context);
        }
    }

    private static OrderItemType MapToOrderItemType(CartItemType type) => type switch
    {
        CartItemType.Product => OrderItemType.Product,
        CartItemType.Variant => OrderItemType.Variant,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    private static OrderItemType MapToOrderItemType(PriceItemType type) => type switch
    {
        PriceItemType.Product => OrderItemType.Product,
        PriceItemType.Variant => OrderItemType.Variant,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
