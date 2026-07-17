using BuildingBlocks.Application.Messaging;
using MediatR;
using Pricing.Application.Common;

namespace Pricing.Application.Commands.CouponUsage.CommitCouponUsage;

/// <summary>
/// Records that a coupon was actually used on a confirmed order, for total/per-user usage-limit
/// enforcement. No IRequireRole — like Inventory's Reserve/Confirm/Release and Cart's Merge command,
/// this has no caller yet and is meant to be invoked in-process by the future Order module once an
/// order is confirmed, using the same discount amounts CalculateMyOrderPrice/CalculateGuestOrderPrice
/// returned at checkout time.
/// </summary>
public record CommitCouponUsageCommand(
    IReadOnlyCollection<AppliedCouponDto> AppliedCoupons,
    Guid? CustomerId,
    Guid? GuestCustomerId,
    Guid OrderId) : ICommand<Unit>;
