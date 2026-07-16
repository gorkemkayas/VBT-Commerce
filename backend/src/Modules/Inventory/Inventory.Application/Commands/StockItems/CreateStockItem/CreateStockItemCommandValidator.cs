using FluentValidation;

namespace Inventory.Application.Commands.StockItems.CreateStockItem;

public class CreateStockItemCommandValidator : AbstractValidator<CreateStockItemCommand>
{
    public CreateStockItemCommandValidator()
    {
        RuleFor(x => x.SellableItemId).NotEmpty();
        RuleFor(x => x.SellableItemType).IsInEnum();
        RuleFor(x => x.InitialQuantity).GreaterThanOrEqualTo(0);
    }
}
