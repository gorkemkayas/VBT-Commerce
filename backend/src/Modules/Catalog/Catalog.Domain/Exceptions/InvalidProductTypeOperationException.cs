using BuildingBlocks.Domain;

namespace Catalog.Domain.Exceptions;

public class InvalidProductTypeOperationException(string message) : DomainException(message)
{
}
