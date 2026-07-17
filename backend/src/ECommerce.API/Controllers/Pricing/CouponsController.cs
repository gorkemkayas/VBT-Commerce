using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pricing.Application.Commands.Coupons.CreateCoupon;
using Pricing.Application.Commands.Coupons.DeactivateCoupon;
using Pricing.Application.Commands.Coupons.UpdateCoupon;
using Pricing.Application.Common;
using Pricing.Application.Queries.Coupons.GetCouponByCode;
using Pricing.Application.Queries.Coupons.GetCouponsList;

namespace ECommerce.API.Controllers.Pricing;

[ApiController]
[Route("api/admin/coupons")]
public class CouponsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCoupon(CreateCouponRequest request, CancellationToken cancellationToken)
    {
        var couponId = await sender.Send(
            new CreateCouponCommand(
                request.Code,
                request.DiscountType,
                request.DiscountValue,
                request.MaxDiscountAmount,
                request.MinCartAmount,
                request.ScopeType,
                request.ScopeReferenceId,
                request.StartDate,
                request.EndDate,
                request.TotalUsageLimit,
                request.PerUserUsageLimit),
            cancellationToken);

        return CreatedAtAction(nameof(GetCouponByCode), new { code = request.Code }, couponId);
    }

    [HttpPut("{couponId:guid}")]
    public async Task<IActionResult> UpdateCoupon(Guid couponId, UpdateCouponRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateCouponCommand(
                couponId,
                request.DiscountType,
                request.DiscountValue,
                request.MaxDiscountAmount,
                request.MinCartAmount,
                request.ScopeType,
                request.ScopeReferenceId,
                request.StartDate,
                request.EndDate,
                request.TotalUsageLimit,
                request.PerUserUsageLimit),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{couponId:guid}/deactivate")]
    public async Task<IActionResult> DeactivateCoupon(Guid couponId, CancellationToken cancellationToken)
    {
        await sender.Send(new DeactivateCouponCommand(couponId), cancellationToken);
        return NoContent();
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<CouponDto>> GetCouponByCode(string code, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCouponByCodeQuery(code), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CouponDto>>> GetCouponsList(
        [FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetCouponsListQuery(
            pageNumber == 0 ? 1 : pageNumber,
            pageSize == 0 ? 20 : pageSize);

        var result = await sender.Send(query, cancellationToken);
        return Ok(result);
    }
}
