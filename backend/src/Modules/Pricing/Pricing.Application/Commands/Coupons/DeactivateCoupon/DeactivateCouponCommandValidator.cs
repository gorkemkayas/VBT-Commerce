using FluentValidation;

namespace Pricing.Application.Commands.Coupons.DeactivateCoupon;

public class DeactivateCouponCommandValidator : AbstractValidator<DeactivateCouponCommand>
{
    public DeactivateCouponCommandValidator()
    {
        RuleFor(x => x.CouponId).NotEmpty();
    }
}
