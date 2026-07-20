using BuildingBlocks.Domain;

namespace Order.Domain.Exceptions;

public class OrderInsufficientStockException : DomainException
{
    public OrderInsufficientStockException(string reason)
        : base($"Insufficient stock: {reason}")
    {
    }
}
