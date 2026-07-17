using FluentValidation;

namespace Order.Application.Queries.Me.GetMyOrdersList;

public class GetMyOrdersListQueryValidator : AbstractValidator<GetMyOrdersListQuery>
{
    public GetMyOrdersListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
