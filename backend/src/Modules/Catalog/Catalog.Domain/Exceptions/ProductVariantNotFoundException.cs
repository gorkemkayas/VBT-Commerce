using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class ProductVariantNotFoundException(Guid variantId) : DomainException($"Product variant '{variantId}' was not found.")
{
}
