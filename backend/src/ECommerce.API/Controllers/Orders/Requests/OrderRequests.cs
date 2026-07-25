namespace ECommerce.API.Controllers.Orders.Requests;

public record PlaceMyOrderRequest(
    Guid AddressId,
    Guid? BillingAddressId,
    Guid ShippingCompanyId,
    IReadOnlyCollection<string> CouponCodes,
    string CardHolderName,
    string CardNumber,
    string CardExpireMonth,
    string CardExpireYear,
    string CardCvc,
    string BuyerIdentityNumber);

public record PlaceGuestOrderRequest(
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
    string? BillingRecipientName,
    string? BillingPhoneNumber,
    string? BillingCountry,
    string? BillingCity,
    string? BillingDistrict,
    string? BillingPostalCode,
    string? BillingAddressLine1,
    string? BillingAddressLine2,
    string CardHolderName,
    string CardNumber,
    string CardExpireMonth,
    string CardExpireYear,
    string CardCvc,
    string BuyerIdentityNumber);

public record CancelOrderRequest(string? Reason);
