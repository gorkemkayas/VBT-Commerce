using BuildingBlocks.Domain;

namespace Inventory.Domain.Exceptions;

public class DuplicateStockItemException(Guid sellableItemId)
    : DomainException($"A stock item already exists for sellable item '{sellableItemId}'.")
{
}
