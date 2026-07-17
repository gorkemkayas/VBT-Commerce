using FluentValidation;

namespace Shipping.Application.Queries.ShippingCompanies.GetShippingCompanyById;

public class GetShippingCompanyByIdQueryValidator : AbstractValidator<GetShippingCompanyByIdQuery>
{
    public GetShippingCompanyByIdQueryValidator()
    {
        RuleFor(x => x.ShippingCompanyId).NotEmpty();
    }
}
