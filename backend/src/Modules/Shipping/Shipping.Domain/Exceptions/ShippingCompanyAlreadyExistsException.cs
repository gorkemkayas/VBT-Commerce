using BuildingBlocks.Domain;

namespace Shipping.Domain.Exceptions;

public class ShippingCompanyAlreadyExistsException : DomainException
{
    public ShippingCompanyAlreadyExistsException(string name)
        : base($"A shipping company named '{name}' already exists.")
    {
    }
}
