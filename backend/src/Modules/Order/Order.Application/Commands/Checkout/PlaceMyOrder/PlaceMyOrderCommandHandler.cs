using BuildingBlocks.Application.Security;
using Cart.Application.Commands.Me.ClearMyCart;
using Cart.Domain.Enums;
using MediatR;
using Order.Application.Common;
using Order.Application.Integrations;
using Order.Application.Services;
using Order.Domain.Exceptions;
using Pricing.Application.Common;
using Pricing.Application.Queries.Calculate.CalculateMyOrderPrice;
using Pricing.Domain.Enums;

namespace Order.Application.Commands.Checkout.PlaceMyOrder;

public class PlaceMyOrderCommandHandler(
    ICurrentUserService currentUserService,
    ICustomerIntegrationService customerIntegrationService,
    ICartIntegrationService cartIntegrationService,
    ISender sender,
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

        var priceResult = await sender.Send(new CalculateMyOrderPriceQuery(priceItems, request.CouponCodes), cancellationToken);

        var addressSnapshot = new OrderAddressSnapshot(
            address.RecipientName, address.PhoneNumber, address.Country, address.City,
            address.District, address.PostalCode, address.AddressLine1, address.AddressLine2);

        return await orderOperations.PlaceOrderAsync(
            OrderOwnerKey.ForUser(userId),
            cart.Items,
            addressSnapshot,
            request.ShippingCompanyId,
            priceResult,
            ct => sender.Send(new ClearMyCartCommand(), ct),
            cancellationToken);
    }

    private static PriceItemType MapToPriceItemType(CartItemType type) => type switch
    {
        CartItemType.Product => PriceItemType.Product,
        CartItemType.Variant => PriceItemType.Variant,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
