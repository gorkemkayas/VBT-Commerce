using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class ProductVariantAttributeNotFoundException(Guid variantAttributeId)
    : DomainException($"Product variant attribute '{variantAttributeId}' was not found.")
{
}
