using BuildingBlocks.Domain;

namespace Identity.Domain.Exceptions;

public class EmailAlreadyRegisteredException(string email) : DomainException($"Email '{email}' is already registered.")
{
}
