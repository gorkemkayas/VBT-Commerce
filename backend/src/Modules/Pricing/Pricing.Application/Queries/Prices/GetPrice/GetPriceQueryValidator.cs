using FluentValidation;

namespace Pricing.Application.Queries.Prices.GetPrice;

public class GetPriceQueryValidator : AbstractValidator<GetPriceQuery>
{
    public GetPriceQueryValidator()
    {
        RuleFor(x => x.SellableItemId).NotEmpty();
        RuleFor(x => x.SellableItemType).IsInEnum();
    }
}
