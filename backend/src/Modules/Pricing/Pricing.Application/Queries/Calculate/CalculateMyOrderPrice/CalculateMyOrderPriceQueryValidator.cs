using FluentValidation;

namespace Pricing.Application.Queries.Calculate.CalculateMyOrderPrice;

public class CalculateMyOrderPriceQueryValidator : AbstractValidator<CalculateMyOrderPriceQuery>
{
    public CalculateMyOrderPriceQueryValidator()
    {
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
