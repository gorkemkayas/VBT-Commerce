using BuildingBlocks.Domain;

namespace Cart.Domain.Exceptions;

public class CartSellableItemNotFoundException : DomainException
{
    public CartSellableItemNotFoundException(Guid sellableItemId)
        : base($"Product or variant '{sellableItemId}' was not found.")
    {
    }
}
