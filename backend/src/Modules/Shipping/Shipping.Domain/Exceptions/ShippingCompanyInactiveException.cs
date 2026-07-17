using BuildingBlocks.Domain;

namespace Shipping.Domain.Exceptions;

public class ShippingCompanyInactiveException : DomainException
{
    public ShippingCompanyInactiveException(Guid shippingCompanyId)
        : base($"Shipping company '{shippingCompanyId}' is inactive and cannot be used.")
    {
    }
}
