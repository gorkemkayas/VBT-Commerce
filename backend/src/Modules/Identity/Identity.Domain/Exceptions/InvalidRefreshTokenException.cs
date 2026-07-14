using BuildingBlocks.Domain;

namespace Identity.Domain.Exceptions;

public class InvalidRefreshTokenException(string message) : DomainException(message)
{
}
