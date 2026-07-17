using BuildingBlocks.Domain;

namespace Pricing.Domain.Exceptions;

public class PricingGuestCustomerNotFoundException : DomainException
{
    public PricingGuestCustomerNotFoundException(Guid guestCustomerId)
        : base($"Guest customer '{guestCustomerId}' was not found.")
    {
    }
}
