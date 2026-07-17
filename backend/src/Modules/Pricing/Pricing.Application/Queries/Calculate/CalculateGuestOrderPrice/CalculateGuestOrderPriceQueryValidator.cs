using FluentValidation;

namespace Pricing.Application.Queries.Calculate.CalculateGuestOrderPrice;

public class CalculateGuestOrderPriceQueryValidator : AbstractValidator<CalculateGuestOrderPriceQuery>
{
    public CalculateGuestOrderPriceQueryValidator()
    {
        RuleFor(x => x.GuestCustomerId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.SellableItemId).NotEmpty();
            item.RuleFor(i => i.SellableItemType).IsInEnum();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
        RuleForEach(x => x.CouponCodes).NotEmpty();
    }
}
