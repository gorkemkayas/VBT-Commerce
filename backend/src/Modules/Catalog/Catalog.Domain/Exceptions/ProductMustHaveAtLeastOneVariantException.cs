using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class ProductMustHaveAtLeastOneVariantException(Guid productId)
    : DomainException($"Product '{productId}' is of type 'Variant' and must have at least one active variant before it can be activated.")
{
}
