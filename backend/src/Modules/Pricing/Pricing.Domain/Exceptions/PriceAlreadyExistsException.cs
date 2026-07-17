using BuildingBlocks.Domain;
using Pricing.Domain.Enums;

namespace Pricing.Domain.Exceptions;

public class PriceAlreadyExistsException : DomainException
{
    public PriceAlreadyExistsException(Guid sellableItemId, PriceItemType sellableItemType)
        : base($"A price already exists for {sellableItemType} '{sellableItemId}'.")
    {
    }
}
