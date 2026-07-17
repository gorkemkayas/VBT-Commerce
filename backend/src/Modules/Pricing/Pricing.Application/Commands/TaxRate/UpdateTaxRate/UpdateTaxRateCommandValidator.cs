using FluentValidation;

namespace Pricing.Application.Commands.TaxRate.UpdateTaxRate;

public class UpdateTaxRateCommandValidator : AbstractValidator<UpdateTaxRateCommand>
{
    public UpdateTaxRateCommandValidator()
    {
        RuleFor(x => x.Rate).InclusiveBetween(0, 100);
    }
}
