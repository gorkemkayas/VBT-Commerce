using BuildingBlocks.Domain;

namespace Shipping.Domain.Exceptions;

public class ShippingCompanyNotFoundException : DomainException
{
    public ShippingCompanyNotFoundException(Guid shippingCompanyId)
        : base($"Shipping company '{shippingCompanyId}' was not found.")
    {
    }
}
