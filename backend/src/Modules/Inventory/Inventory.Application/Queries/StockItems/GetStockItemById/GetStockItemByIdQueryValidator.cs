using FluentValidation;

namespace Inventory.Application.Queries.StockItems.GetStockItemById;

public class GetStockItemByIdQueryValidator : AbstractValidator<GetStockItemByIdQuery>
{
    public GetStockItemByIdQueryValidator()
    {
        RuleFor(x => x.StockItemId).NotEmpty();
    }
}
