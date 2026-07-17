using FluentValidation;

namespace Shipping.Application.Queries.Shipments.GetShipmentById;

public class GetShipmentByIdQueryValidator : AbstractValidator<GetShipmentByIdQuery>
{
    public GetShipmentByIdQueryValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();
    }
}
