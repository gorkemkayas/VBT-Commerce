using BuildingBlocks.Domain;

namespace Order.Domain.Exceptions;

public class OrderAddressNotFoundException : DomainException
{
    public OrderAddressNotFoundException(Guid addressId)
        : base($"Address '{addressId}' was not found.")
    {
    }
}
