using BuildingBlocks.Domain;

namespace Customer.Domain.Exceptions;

public class CustomerProfileAlreadyExistsException(Guid userId)
    : DomainException($"A customer profile already exists for user '{userId}'.")
{
}
