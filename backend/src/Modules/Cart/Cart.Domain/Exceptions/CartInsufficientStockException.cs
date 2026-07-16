using BuildingBlocks.Domain;

namespace Cart.Domain.Exceptions;

public class CartInsufficientStockException : DomainException
{
    public CartInsufficientStockException(Guid sellableItemId, int requestedQuantity, int availableQuantity)
        : base($"Requested quantity {requestedQuantity} for '{sellableItemId}' exceeds available stock ({availableQuantity}).")
    {
    }
}
