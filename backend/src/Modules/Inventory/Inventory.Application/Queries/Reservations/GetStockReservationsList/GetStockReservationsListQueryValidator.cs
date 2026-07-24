using FluentValidation;

namespace Inventory.Application.Queries.Reservations.GetStockReservationsList;

public class GetStockReservationsListQueryValidator : AbstractValidator<GetStockReservationsListQuery>
{
    public GetStockReservationsListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
