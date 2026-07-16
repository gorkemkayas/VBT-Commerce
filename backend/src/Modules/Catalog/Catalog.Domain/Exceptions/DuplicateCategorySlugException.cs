using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class DuplicateCategorySlugException(string slug) : DomainException($"A category with slug '{slug}' already exists.")
{
}
