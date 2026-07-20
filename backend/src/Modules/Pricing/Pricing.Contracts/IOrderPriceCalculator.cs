namespace Pricing.Contracts;

/// <summary>
/// The Pricing module's outbound write contract for checkout (architecture.md §3). Other modules
/// (chiefly Order) depend on this instead of Pricing's internal Application queries/commands.
/// </summary>
public interface IOrderPriceCalculator
{
    Task<PriceCalculationResultDto> CalculateForCustomerAsync(
        Guid customerId, IReadOnlyCollection<PriceCalculationItem> items, IReadOnlyCollection<string> couponCodes, CancellationToken cancellationToken);

    Task<PriceCalculationResultDto> CalculateForGuestAsync(
        Guid guestCustomerId, IReadOnlyCollection<PriceCalculationItem> items, IReadOnlyCollection<string> couponCodes, CancellationToken cancellationToken);

    Task CommitCouponUsageAsync(
        IReadOnlyCollection<AppliedCouponDto> appliedCoupons, Guid? customerId, Guid? guestCustomerId, Guid orderId, CancellationToken cancellationToken);
}
