using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class DuplicateVariantOptionCombinationException(Guid productId)
    : DomainException($"Product '{productId}' already has a variant with this exact combination of option values.")
{
}
