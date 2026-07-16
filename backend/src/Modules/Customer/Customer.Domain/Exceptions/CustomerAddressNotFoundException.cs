using BuildingBlocks.Domain;

namespace Customer.Domain.Exceptions;

public class CustomerAddressNotFoundException(Guid addressId) : DomainException($"Customer address '{addressId}' was not found.")
{
}
