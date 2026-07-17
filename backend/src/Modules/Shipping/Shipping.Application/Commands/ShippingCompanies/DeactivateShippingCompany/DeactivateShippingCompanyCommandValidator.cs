using FluentValidation;

namespace Shipping.Application.Commands.ShippingCompanies.DeactivateShippingCompany;

public class DeactivateShippingCompanyCommandValidator : AbstractValidator<DeactivateShippingCompanyCommand>
{
    public DeactivateShippingCompanyCommandValidator()
    {
        RuleFor(x => x.ShippingCompanyId).NotEmpty();
    }
}
