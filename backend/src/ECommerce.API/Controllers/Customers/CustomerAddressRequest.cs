namespace ECommerce.API.Controllers.Customers;

public record AddCustomerAddressRequest(
    string Label,
    string RecipientName,
    string PhoneNumber,
    string Country,
    string City,
    string District,
    string PostalCode,
    string AddressLine1,
    string? AddressLine2,
    bool IsDefault);

public record UpdateCustomerAddressRequest(
    string Label,
    string RecipientName,
    string PhoneNumber,
    string Country,
    string City,
    string District,
    string PostalCode,
    string AddressLine1,
    string? AddressLine2,
    bool IsDefault);
