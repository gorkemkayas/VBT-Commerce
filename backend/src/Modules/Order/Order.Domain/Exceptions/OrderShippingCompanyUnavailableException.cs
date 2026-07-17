using BuildingBlocks.Domain;

namespace Order.Domain.Exceptions;

public class OrderShippingCompanyUnavailableException : DomainException
{
    public OrderShippingCompanyUnavailableException(Guid shippingCompanyId)
        : base($"Shipping company '{shippingCompanyId}' is not available.")
    {
    }
}
