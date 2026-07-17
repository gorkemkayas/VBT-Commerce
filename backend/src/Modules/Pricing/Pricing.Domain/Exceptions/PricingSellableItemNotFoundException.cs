using BuildingBlocks.Domain;
using Pricing.Domain.Enums;

namespace Pricing.Domain.Exceptions;

public class PricingSellableItemNotFoundException : DomainException
{
    public PricingSellableItemNotFoundException(Guid sellableItemId, PriceItemType sellableItemType)
        : base($"{sellableItemType} '{sellableItemId}' was not found in the catalog.")
    {
    }
}
