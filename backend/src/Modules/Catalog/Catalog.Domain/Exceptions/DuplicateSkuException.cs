using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class DuplicateSkuException(string sku) : DomainException($"A variant with SKU '{sku}' already exists for this product.")
{
}
