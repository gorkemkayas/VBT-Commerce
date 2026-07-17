namespace Pricing.Application.Integrations;

/// <summary>
/// Pricing's own view of what it needs from the Customer module — verifying a guest checkout's
/// GuestCustomerId is a real record before tracking coupon usage against it (unlike Cart's raw
/// AnonymousId, which is just a correlation key, coupon usage-limit enforcement needs a real identity).
/// Implemented in Pricing.Infrastructure against Customer's ICustomerDirectoryService.
/// </summary>
public interface ICustomerIntegrationService
{
    Task<bool> GuestCustomerExistsAsync(Guid guestCustomerId, CancellationToken cancellationToken);
}
