using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class CategoryCannotBeOwnParentException(Guid categoryId)
    : DomainException($"Category '{categoryId}' cannot be its own parent.")
{
}
