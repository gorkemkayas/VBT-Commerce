using FluentValidation;

namespace Shipping.Application.Queries.Shipments.GetShipmentByOrderId;

public class GetShipmentByOrderIdQueryValidator : AbstractValidator<GetShipmentByOrderIdQuery>
{
    public GetShipmentByOrderIdQueryValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
