using BuildingBlocks.Domain;

namespace Order.Domain.Exceptions;

public class OrderGuestCustomerNotFoundException : DomainException
{
    public OrderGuestCustomerNotFoundException(Guid guestCustomerId)
        : base($"Guest customer '{guestCustomerId}' was not found.")
    {
    }
}
