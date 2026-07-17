using FluentValidation;

namespace Order.Application.Queries.Guest.GetGuestOrderById;

public class GetGuestOrderByIdQueryValidator : AbstractValidator<GetGuestOrderByIdQuery>
{
    public GetGuestOrderByIdQueryValidator()
    {
        RuleFor(x => x.GuestCustomerId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
