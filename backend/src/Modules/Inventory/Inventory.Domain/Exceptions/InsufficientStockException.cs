using BuildingBlocks.Domain;

namespace Inventory.Domain.Exceptions;

public class InsufficientStockException(Guid stockItemId, int requestedQuantity, int availableQuantity)
    : DomainException($"Insufficient stock for item '{stockItemId}': requested {requestedQuantity}, available {availableQuantity}.")
{
}
