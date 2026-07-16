using FluentValidation;

namespace Inventory.Application.Queries.StockItems.GetStockItemsList;

public class GetStockItemsListQueryValidator : AbstractValidator<GetStockItemsListQuery>
{
    public GetStockItemsListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
