using BuildingBlocks.Domain;

namespace Pricing.Domain.Exceptions;

public class InvalidPriceAmountException : DomainException
{
    public InvalidPriceAmountException(decimal amount)
        : base($"Price amount '{amount}' is invalid. Amount must be greater than zero.")
    {
    }
}
