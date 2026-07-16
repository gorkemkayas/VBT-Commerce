using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class VariantAttributeMismatchException(Guid productId)
    : DomainException($"The supplied option values do not match exactly the variant attributes defined for product '{productId}'.")
{
}
