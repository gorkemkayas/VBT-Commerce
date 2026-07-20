using BuildingBlocks.Application.Security;
using Cart.Domain.Enums;
using MediatR;
using Order.Application.Common;
using Order.Application.Integrations;
using Order.Application.Services;
using Order.Domain.Exceptions;
using Payment.Application.Gateway;
using Pricing.Application.Common;
using Pricing.Application.Queries.Calculate.CalculateGuestOrderPrice;
using Pricing.Domain.Enums;

namespace Order.Application.Commands.Checkout.PlaceGuestOrder;

public class PlaceGuestOrderCommandHandler(
    ICurrentUserService currentUserService,
    ICustomerIntegrationService customerIntegrationService,
    ICartIntegrationService cartIntegrationService,
    ISender sender,
    OrderOperations orderOperations) : IRequestHandler<PlaceGuestOrderCommand, Guid>
{
    public async Task<Guid> Handle(PlaceGuestOrderCommand request, CancellationToken cancellationToken)
    {
        var guest = await customerIntegrationService.GetGuestCustomerAsync(request.GuestCustomerId, cancellationToken)
            ?? throw new OrderGuestCustomerNotFoundException(request.GuestCustomerId);

        var cart = await cartIntegrationService.GetCartByAnonymousIdAsync(request.AnonymousId, cancellationToken);
        if (cart is null || cart.Items.Count == 0)
            throw new OrderCartEmptyException();

        var priceItems = cart.Items
            .Select(i => new PriceCalculationItem(i.SellableItemId, MapToPriceItemType(i.SellableItemType), i.Quantity))
            .ToList();

        var priceResult = await sender.Send(
            new CalculateGuestOrderPriceQuery(request.GuestCustomerId, priceItems, request.CouponCodes), cancellationToken);

        var addressSnapshot = new OrderAddressSnapshot(
            request.RecipientName, request.PhoneNumber, request.Country, request.City,
            request.District, request.PostalCode, request.AddressLine1, request.AddressLine2);

        var card = new IyzicoCardInfo(request.CardHolderName, request.CardNumber, request.CardExpireMonth, request.CardExpireYear, request.CardCvc);
        var buyer = new IyzicoBuyerInfo(
            guest.FirstName,
            guest.LastName,
            guest.Email,
            request.BuyerIdentityNumber,
            request.PhoneNumber,
            currentUserService.IpAddress ?? "0.0.0.0");

        return await orderOperations.PlaceOrderAsync(
            OrderOwnerKey.ForGuestCustomer(request.GuestCustomerId),
            cart.Items,
            addressSnapshot,
            request.ShippingCompanyId,
            priceResult,
            card,
            buyer,
            ct => cartIntegrationService.ClearByAnonymousIdAsync(request.AnonymousId, ct),
            cancellationToken);
    }

    private static PriceItemType MapToPriceItemType(CartItemType type) => type switch
    {
        CartItemType.Product => PriceItemType.Product,
        CartItemType.Variant => PriceItemType.Variant,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
