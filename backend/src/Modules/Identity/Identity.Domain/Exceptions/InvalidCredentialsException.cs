using BuildingBlocks.Domain;

namespace Identity.Domain.Exceptions;

public class InvalidCredentialsException() : DomainException("Email or password is incorrect.")
{
}
