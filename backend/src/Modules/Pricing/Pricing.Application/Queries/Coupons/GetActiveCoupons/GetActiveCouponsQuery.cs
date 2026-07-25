using BuildingBlocks.Application.Messaging;
using Pricing.Application.Common;

namespace Pricing.Application.Queries.Coupons.GetActiveCoupons;

/// <summary>
/// Public, unauthenticated counterpart to GetCouponsListQuery (Admin-only) — used to surface
/// currently redeemable coupon codes to storefront visitors (e.g. a homepage promo ticker).
/// </summary>
public record GetActiveCouponsQuery : IQuery<IReadOnlyList<CouponDto>>;
