using BuildingBlocks.Domain;

namespace Pricing.Domain.Exceptions;

public class InvalidTaxRateException : DomainException
{
    public InvalidTaxRateException(decimal rate)
        : base($"Tax rate '{rate}' is invalid. Rate must be between 0 and 100.")
    {
    }
}
