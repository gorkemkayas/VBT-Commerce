using FluentValidation;

namespace Order.Application.Queries.Me.GetMyOrderShipment;

public class GetMyOrderShipmentQueryValidator : AbstractValidator<GetMyOrderShipmentQuery>
{
    public GetMyOrderShipmentQueryValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
