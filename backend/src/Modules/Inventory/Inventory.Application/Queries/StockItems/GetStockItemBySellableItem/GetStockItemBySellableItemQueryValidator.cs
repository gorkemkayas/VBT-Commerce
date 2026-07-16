using FluentValidation;

namespace Inventory.Application.Queries.StockItems.GetStockItemBySellableItem;

public class GetStockItemBySellableItemQueryValidator : AbstractValidator<GetStockItemBySellableItemQuery>
{
    public GetStockItemBySellableItemQueryValidator()
    {
        RuleFor(x => x.SellableItemId).NotEmpty();
        RuleFor(x => x.SellableItemType).IsInEnum();
    }
}
