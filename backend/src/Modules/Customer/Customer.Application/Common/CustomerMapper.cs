using Customer.Domain.Entities;

namespace Customer.Application.Common;

public static class CustomerMapper
{
    public static CustomerDto ToDto(CustomerProfile customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.UserId,
            customer.PhoneNumber,
            customer.DateOfBirth,
            customer.Addresses
                .Select(a => new CustomerAddressDto(
                    a.Id, a.Label, a.RecipientName, a.PhoneNumber, a.Country, a.City,
                    a.District, a.PostalCode, a.AddressLine1, a.AddressLine2, a.IsDefault))
                .ToList());
    }
}
