using BuildingBlocks.Domain;

namespace Order.Domain.Exceptions;

public class OrderCartEmptyException : DomainException
{
    public OrderCartEmptyException()
        : base("Cannot place an order from an empty cart.")
    {
    }
}
