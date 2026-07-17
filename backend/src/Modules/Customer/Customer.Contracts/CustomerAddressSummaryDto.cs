namespace Customer.Contracts;

public record CustomerAddressSummaryDto(
    Guid Id,
    string RecipientName,
    string PhoneNumber,
    string Country,
    string City,
    string District,
    string PostalCode,
    string AddressLine1,
    string? AddressLine2);
