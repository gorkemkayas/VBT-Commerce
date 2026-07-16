using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class ProductImageNotFoundException(Guid imageId) : DomainException($"Product image '{imageId}' was not found.")
{
}
