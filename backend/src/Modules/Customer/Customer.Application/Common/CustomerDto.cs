namespace Customer.Application.Common;

public record CustomerAddressDto(
    Guid Id,
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

public record CustomerDto(
    Guid Id,
    Guid UserId,
    string? PhoneNumber,
    DateOnly? DateOfBirth,
    IReadOnlyCollection<CustomerAddressDto> Addresses);

public record CustomerListItemDto(Guid Id, Guid UserId, string? PhoneNumber, int AddressCount);

public record GuestCustomerDto(Guid Id, string FirstName, string LastName, string Email, string PhoneNumber);
