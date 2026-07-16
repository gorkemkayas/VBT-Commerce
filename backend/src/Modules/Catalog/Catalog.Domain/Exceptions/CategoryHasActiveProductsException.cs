using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class CategoryHasActiveProductsException(Guid categoryId)
    : DomainException($"Category '{categoryId}' has active products and cannot be deactivated. Move or deactivate its products first.")
{
}
