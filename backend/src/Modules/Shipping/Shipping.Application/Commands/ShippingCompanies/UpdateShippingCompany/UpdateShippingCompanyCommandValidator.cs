using FluentValidation;

namespace Shipping.Application.Commands.ShippingCompanies.UpdateShippingCompany;

public class UpdateShippingCompanyCommandValidator : AbstractValidator<UpdateShippingCompanyCommand>
{
    public UpdateShippingCompanyCommandValidator()
    {
        RuleFor(x => x.ShippingCompanyId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Fee).GreaterThan(0);
    }
}
