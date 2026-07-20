using Order.Application.Integrations;
using Pricing.Contracts;

namespace Order.Infrastructure.Integrations;

/// <summary>
/// Adapts Order's own <see cref="IPricingIntegrationService"/> to Pricing's actual contract
/// (<see cref="IOrderPriceCalculator"/>), so Order.Application never references Pricing.Application directly.
/// </summary>
public class PricingIntegrationService(IOrderPriceCalculator orderPriceCalculator) : IPricingIntegrationService
{
    public Task<PriceCalculationResultDto> CalculateForCustomerAsync(
        Guid customerId, IReadOnlyCollection<PriceCalculationItem> items, IReadOnlyCollection<string> couponCodes, CancellationToken cancellationToken)
        => orderPriceCalculator.CalculateForCustomerAsync(customerId, items, couponCodes, cancellationToken);

    public Task<PriceCalculationResultDto> CalculateForGuestAsync(
        Guid guestCustomerId, IReadOnlyCollection<PriceCalculationItem> items, IReadOnlyCollection<string> couponCodes, CancellationToken cancellationToken)
        => orderPriceCalculator.CalculateForGuestAsync(guestCustomerId, items, couponCodes, cancellationToken);

    public Task CommitCouponUsageAsync(
        IReadOnlyCollection<AppliedCouponDto> appliedCoupons, Guid? customerId, Guid? guestCustomerId, Guid orderId, CancellationToken cancellationToken)
        => orderPriceCalculator.CommitCouponUsageAsync(appliedCoupons, customerId, guestCustomerId, orderId, cancellationToken);
}
