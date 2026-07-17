using BuildingBlocks.Domain;

namespace Order.Domain.Exceptions;

public class OrderCustomerProfileNotFoundException : DomainException
{
    public OrderCustomerProfileNotFoundException(Guid userId)
        : base($"No customer profile exists for user '{userId}'. Create one before placing an order.")
    {
    }
}
