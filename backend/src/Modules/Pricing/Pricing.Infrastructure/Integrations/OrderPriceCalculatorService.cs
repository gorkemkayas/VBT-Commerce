using MediatR;
using Pricing.Application.Commands.CouponUsage.CommitCouponUsage;
using Pricing.Application.Queries.Calculate.CalculateCustomerOrderPrice;
using Pricing.Application.Queries.Calculate.CalculateGuestOrderPrice;
using Pricing.Contracts;

namespace Pricing.Infrastructure.Integrations;

/// <summary>
/// Implementation of Pricing's outbound write contract (<see cref="IOrderPriceCalculator"/>).
/// Delegates to the same MediatR queries/commands used internally, instead of duplicating
/// PriceCalculationService's tax/coupon logic.
/// </summary>
public class OrderPriceCalculatorService(ISender sender) : IOrderPriceCalculator
{
    public Task<PriceCalculationResultDto> CalculateForCustomerAsync(
        Guid customerId, IReadOnlyCollection<PriceCalculationItem> items, IReadOnlyCollection<string> couponCodes, CancellationToken cancellationToken)
        => sender.Send(new CalculateCustomerOrderPriceQuery(customerId, items, couponCodes), cancellationToken);

    public Task<PriceCalculationResultDto> CalculateForGuestAsync(
        Guid guestCustomerId, IReadOnlyCollection<PriceCalculationItem> items, IReadOnlyCollection<string> couponCodes, CancellationToken cancellationToken)
        => sender.Send(new CalculateGuestOrderPriceQuery(guestCustomerId, items, couponCodes), cancellationToken);

    public async Task CommitCouponUsageAsync(
        IReadOnlyCollection<AppliedCouponDto> appliedCoupons, Guid? customerId, Guid? guestCustomerId, Guid orderId, CancellationToken cancellationToken)
    {
        await sender.Send(new CommitCouponUsageCommand(appliedCoupons, customerId, guestCustomerId, orderId), cancellationToken);
    }
}
