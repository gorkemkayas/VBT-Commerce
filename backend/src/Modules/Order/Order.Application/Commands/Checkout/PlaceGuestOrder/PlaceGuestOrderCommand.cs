using BuildingBlocks.Application.Messaging;

namespace Order.Application.Commands.Checkout.PlaceGuestOrder;

public record PlaceGuestOrderCommand(
    Guid GuestCustomerId,
    Guid AnonymousId,
    Guid ShippingCompanyId,
    IReadOnlyCollection<string> CouponCodes,
    string RecipientName,
    string PhoneNumber,
    string Country,
    string City,
    string District,
    string PostalCode,
    string AddressLine1,
    string? AddressLine2,
    string CardHolderName,
    string CardNumber,
    string CardExpireMonth,
    string CardExpireYear,
    string CardCvc,
    string BuyerIdentityNumber) : ICommand<Guid>;
