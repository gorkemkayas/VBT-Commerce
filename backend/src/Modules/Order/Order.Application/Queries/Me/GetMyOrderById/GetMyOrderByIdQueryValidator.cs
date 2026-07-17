using FluentValidation;

namespace Order.Application.Queries.Me.GetMyOrderById;

public class GetMyOrderByIdQueryValidator : AbstractValidator<GetMyOrderByIdQuery>
{
    public GetMyOrderByIdQueryValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
