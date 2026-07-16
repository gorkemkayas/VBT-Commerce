using BuildingBlocks.Domain;

namespace Customer.Domain.Exceptions;

public class CustomerNotFoundException : DomainException
{
    private CustomerNotFoundException(string message) : base(message)
    {
    }

    public static CustomerNotFoundException ForCustomerId(Guid customerId)
        => new($"Customer '{customerId}' was not found.");

    public static CustomerNotFoundException ForUserId(Guid userId)
        => new($"No customer profile was found for user '{userId}'.");
}
