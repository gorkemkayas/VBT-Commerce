using BuildingBlocks.Domain;

namespace Inventory.Domain.Exceptions;

public class SellableItemNotFoundException(Guid sellableItemId)
    : DomainException($"No product or variant with id '{sellableItemId}' was found in the Catalog module.")
{
}
