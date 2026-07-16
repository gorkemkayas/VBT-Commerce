using FluentValidation;

namespace Customer.Application.Queries.Admin.GetCustomersList;

public class GetCustomersListQueryValidator : AbstractValidator<GetCustomersListQuery>
{
    public GetCustomersListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
