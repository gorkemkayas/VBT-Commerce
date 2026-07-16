using FluentValidation;

namespace Inventory.Application.Commands.StockItems.DecreaseStock;

public class DecreaseStockCommandValidator : AbstractValidator<DecreaseStockCommand>
{
    public DecreaseStockCommandValidator()
    {
        RuleFor(x => x.StockItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
