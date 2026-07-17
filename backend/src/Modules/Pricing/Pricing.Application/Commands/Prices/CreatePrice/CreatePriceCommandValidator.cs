using FluentValidation;

namespace Pricing.Application.Commands.Prices.CreatePrice;

public class CreatePriceCommandValidator : AbstractValidator<CreatePriceCommand>
{
    public CreatePriceCommandValidator()
    {
        RuleFor(x => x.SellableItemId).NotEmpty();
        RuleFor(x => x.SellableItemType).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
