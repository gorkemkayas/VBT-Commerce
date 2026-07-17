using FluentValidation;

namespace Shipping.Application.Queries.Shipments.GetShipmentsList;

public class GetShipmentsListQueryValidator : AbstractValidator<GetShipmentsListQuery>
{
    public GetShipmentsListQueryValidator()
    {
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status is not null);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
