using FluentValidation;

namespace Pricing.Application.Queries.Coupons.GetCouponByCode;

public class GetCouponByCodeQueryValidator : AbstractValidator<GetCouponByCodeQuery>
{
    public GetCouponByCodeQueryValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
    }
}
