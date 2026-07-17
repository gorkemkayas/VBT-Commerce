using FluentValidation;

namespace Shipping.Application.Commands.ShippingCompanies.CreateShippingCompany;

public class CreateShippingCompanyCommandValidator : AbstractValidator<CreateShippingCompanyCommand>
{
    public CreateShippingCompanyCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Fee).GreaterThan(0);
    }
}
