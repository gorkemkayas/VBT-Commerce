using BuildingBlocks.Domain;

namespace Identity.Domain.Exceptions;

public class InvalidOrExpiredResetTokenException()
    : DomainException("This password reset link is invalid, already used, or has expired.")
{
}
