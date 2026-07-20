using BuildingBlocks.Application.Security;
using Cart.Domain.Enums;
using MediatR;
using Order.Application.Common;
using Order.Application.Integrations;
using Order.Application.Services;
using Order.Domain.Exceptions;
using Payment.Application.Gateway;
using Pricing.Contracts;
using Pricing.Domain.Enums;

namespace Order.Application.Commands.Checkout.PlaceMyOrder;

public class PlaceMyOrderCommandHandler(
    ICurrentUserService currentUserService,
    ICustomerIntegrationService customerIntegrationService,
    ICartIntegrationService cartIntegrationService,
    IPricingIntegrationService pricingIntegrationService,
    OrderOperations orderOperations) : IRequestHandler<PlaceMyOrderCommand, Guid>
{
    public async Task<Guid> Handle(PlaceMyOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId!.Value;

        var customer = await customerIntegrationService.GetCustomerByUserIdAsync(userId, cancellationToken)
            ?? throw new OrderCustomerProfileNotFoundException(userId);

        var cart = await cartIntegrationService.GetCartByUserIdAsync(userId, cancellationToken);
        if (cart is null || cart.Items.Count == 0)
            throw new OrderCartEmptyException();

        var address = await customerIntegrationService.GetCustomerAddressAsync(customer.Id, request.AddressId, cancellationToken)
            ?? throw new OrderAddressNotFoundException(request.AddressId);

        var priceItems = cart.Items
            .Select(i => new PriceCalculationItem(i.SellableItemId, MapToPriceItemType(i.SellableItemType), i.Quantity))
            .ToList();

        var priceResult = await pricingIntegrationService.CalculateForCustomerAsync(userId, priceItems, request.CouponCodes, cancellationToken);

        var addressSnapshot = new OrderAddressSnapshot(
            address.RecipientName, address.PhoneNumber, address.Country, address.City,
            address.District, address.PostalCode, address.AddressLine1, address.AddressLine2);

        // Customer has no first/last name field of its own (only Identity does, at registration) —
        // splitting the delivery address's recipient name avoids introducing an Identity.Contracts
        // project just for this; iyzico only uses Name/Surname for fraud scoring, not verification.
        var nameParts = address.RecipientName.Split(' ', 2);
        var card = new IyzicoCardInfo(request.CardHolderName, request.CardNumber, request.CardExpireMonth, request.CardExpireYear, request.CardCvc);
        var buyer = new IyzicoBuyerInfo(
            nameParts[0],
            nameParts.Length > 1 ? nameParts[1] : nameParts[0],
            currentUserService.Email!,
            request.BuyerIdentityNumber,
            address.PhoneNumber,
            currentUserService.IpAddress ?? "0.0.0.0");

        return await orderOperations.PlaceOrderAsync(
            OrderOwnerKey.ForUser(userId),
            cart.Items,
            addressSnapshot,
            request.ShippingCompanyId,
            priceResult,
            card,
            buyer,
            ct => cartIntegrationService.ClearByUserIdAsync(userId, ct),
            cancellationToken);
    }

    private static PriceItemType MapToPriceItemType(CartItemType type) => type switch
    {
        CartItemType.Product => PriceItemType.Product,
        CartItemType.Variant => PriceItemType.Variant,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
