using FluentValidation;

namespace Payment.Application.Queries.GetPaymentsList;

public class GetPaymentsListQueryValidator : AbstractValidator<GetPaymentsListQuery>
{
    public GetPaymentsListQueryValidator()
    {
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status is not null);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
