using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class DuplicateProductSlugException(string slug) : DomainException($"A product with slug '{slug}' already exists.")
{
}
