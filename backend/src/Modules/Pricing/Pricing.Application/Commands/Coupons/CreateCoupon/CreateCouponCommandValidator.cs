using FluentValidation;

namespace Pricing.Application.Commands.Coupons.CreateCoupon;

public class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DiscountType).IsInEnum();
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.MaxDiscountAmount).GreaterThan(0).When(x => x.MaxDiscountAmount is not null);
        RuleFor(x => x.MinCartAmount).GreaterThan(0).When(x => x.MinCartAmount is not null);
        RuleFor(x => x.ScopeType).IsInEnum();
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
        RuleFor(x => x.TotalUsageLimit).GreaterThan(0).When(x => x.TotalUsageLimit is not null);
        RuleFor(x => x.PerUserUsageLimit).GreaterThan(0).When(x => x.PerUserUsageLimit is not null);
    }
}
