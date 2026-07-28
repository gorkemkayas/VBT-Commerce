using FluentValidation;

namespace Pricing.Application.Queries.Prices.GetPrices;

public class GetPricesQueryValidator : AbstractValidator<GetPricesQuery>
{
    public GetPricesQueryValidator()
    {
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.SellableItemId).NotEmpty();
            item.RuleFor(i => i.SellableItemType).IsInEnum();
        });
    }
}
