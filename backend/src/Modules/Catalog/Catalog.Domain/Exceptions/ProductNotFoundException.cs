using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(Guid productId) : base($"Product '{productId}' was not found.")
    {
    }

    public ProductNotFoundException(string slug) : base($"Product with slug '{slug}' was not found.")
    {
    }
}
