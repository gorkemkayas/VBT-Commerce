using BuildingBlocks.Domain;

namespace Inventory.Domain.Exceptions;

public class InvalidStockQuantityException(string message) : DomainException(message)
{
}
