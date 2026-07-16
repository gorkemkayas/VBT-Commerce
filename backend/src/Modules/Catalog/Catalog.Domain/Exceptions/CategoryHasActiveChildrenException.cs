using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class CategoryHasActiveChildrenException(Guid categoryId)
    : DomainException($"Category '{categoryId}' has active subcategories and cannot be deactivated. Deactivate its subcategories first.")
{
}
