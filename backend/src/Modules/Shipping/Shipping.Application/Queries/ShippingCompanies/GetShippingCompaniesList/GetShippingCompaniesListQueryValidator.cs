using FluentValidation;

namespace Shipping.Application.Queries.ShippingCompanies.GetShippingCompaniesList;

public class GetShippingCompaniesListQueryValidator : AbstractValidator<GetShippingCompaniesListQuery>
{
    public GetShippingCompaniesListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
