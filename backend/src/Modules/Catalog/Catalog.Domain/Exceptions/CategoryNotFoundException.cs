using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class CategoryNotFoundException(Guid categoryId) : DomainException($"Category '{categoryId}' was not found.")
{
}
