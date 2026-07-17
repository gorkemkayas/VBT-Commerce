using BuildingBlocks.Domain;
using Pricing.Domain.Enums;

namespace Pricing.Domain.Exceptions;

public class PriceNotFoundException : DomainException
{
    public PriceNotFoundException(Guid sellableItemId, PriceItemType sellableItemType)
        : base($"No price was found for {sellableItemType} '{sellableItemId}'.")
    {
    }

    public PriceNotFoundException(Guid priceId)
        : base($"Price '{priceId}' was not found.")
    {
    }
}
