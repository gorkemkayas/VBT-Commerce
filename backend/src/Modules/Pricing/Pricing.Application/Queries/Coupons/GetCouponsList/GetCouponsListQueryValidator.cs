using FluentValidation;

namespace Pricing.Application.Queries.Coupons.GetCouponsList;

public class GetCouponsListQueryValidator : AbstractValidator<GetCouponsListQuery>
{
    public GetCouponsListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
