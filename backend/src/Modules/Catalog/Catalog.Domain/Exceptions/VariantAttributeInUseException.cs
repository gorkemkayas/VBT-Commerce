using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class VariantAttributeInUseException(Guid variantAttributeId)
    : DomainException($"Variant attribute '{variantAttributeId}' is used by one or more variants and cannot be removed.")
{
}
