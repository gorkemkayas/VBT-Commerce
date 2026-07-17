namespace ECommerce.API.Controllers.Orders;

public record PlaceMyOrderRequest(Guid AddressId, Guid ShippingCompanyId, IReadOnlyCollection<string> CouponCodes);

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
    string? AddressLine2);

public record CancelOrderRequest(string? Reason);
