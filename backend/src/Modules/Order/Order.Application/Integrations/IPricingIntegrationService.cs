using Pricing.Contracts;

namespace Order.Application.Integrations;

public interface IPricingIntegrationService
{
    Task<PriceCalculationResultDto> CalculateForCustomerAsync(
        Guid customerId, IReadOnlyCollection<PriceCalculationItem> items, IReadOnlyCollection<string> couponCodes, CancellationToken cancellationToken);

    Task<PriceCalculationResultDto> CalculateForGuestAsync(
        Guid guestCustomerId, IReadOnlyCollection<PriceCalculationItem> items, IReadOnlyCollection<string> couponCodes, CancellationToken cancellationToken);

    Task CommitCouponUsageAsync(
        IReadOnlyCollection<AppliedCouponDto> appliedCoupons, Guid? customerId, Guid? guestCustomerId, Guid orderId, CancellationToken cancellationToken);
}
