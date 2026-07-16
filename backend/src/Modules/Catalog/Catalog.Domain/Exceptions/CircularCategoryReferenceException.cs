using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class CircularCategoryReferenceException(Guid categoryId, Guid parentCategoryId)
    : DomainException($"Setting '{parentCategoryId}' as the parent of category '{categoryId}' would create a circular reference.")
{
}
