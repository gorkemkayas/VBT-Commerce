using BuildingBlocks.Domain;

namespace Customer.Domain.Exceptions;

public class GuestCustomerNotFoundException(Guid guestCustomerId) : DomainException($"Guest customer '{guestCustomerId}' was not found.")
{
}
