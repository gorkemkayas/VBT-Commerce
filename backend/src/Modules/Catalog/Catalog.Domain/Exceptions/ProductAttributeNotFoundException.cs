using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class ProductAttributeNotFoundException(Guid attributeId) : DomainException($"Product attribute '{attributeId}' was not found.")
{
}
