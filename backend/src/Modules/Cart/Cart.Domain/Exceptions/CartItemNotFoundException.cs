using BuildingBlocks.Domain;

namespace Cart.Domain.Exceptions;

public class CartItemNotFoundException : DomainException
{
    public CartItemNotFoundException(Guid cartItemId)
        : base($"Cart item '{cartItemId}' was not found.")
    {
    }
}
