using FluentValidation;

namespace Pricing.Application.Commands.Prices.UpdatePrice;

public class UpdatePriceCommandValidator : AbstractValidator<UpdatePriceCommand>
{
    public UpdatePriceCommandValidator()
    {
        RuleFor(x => x.PriceId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
