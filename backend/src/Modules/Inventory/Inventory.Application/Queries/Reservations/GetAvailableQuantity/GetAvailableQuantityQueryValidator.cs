using FluentValidation;

namespace Inventory.Application.Queries.Reservations.GetAvailableQuantity;

public class GetAvailableQuantityQueryValidator : AbstractValidator<GetAvailableQuantityQuery>
{
    public GetAvailableQuantityQueryValidator()
    {
        RuleFor(x => x.SellableItemId).NotEmpty();
        RuleFor(x => x.SellableItemType).IsInEnum();
    }
}
