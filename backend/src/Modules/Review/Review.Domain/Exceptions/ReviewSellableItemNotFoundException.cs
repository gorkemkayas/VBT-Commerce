using BuildingBlocks.Domain;

namespace Review.Domain.Exceptions;

public class ReviewSellableItemNotFoundException : DomainException
{
    public ReviewSellableItemNotFoundException(Guid sellableItemId)
        : base($"Sellable item '{sellableItemId}' was not found or is not active.")
    {
    }
}
