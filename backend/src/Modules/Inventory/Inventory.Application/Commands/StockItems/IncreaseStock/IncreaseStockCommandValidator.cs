using FluentValidation;

namespace Inventory.Application.Commands.StockItems.IncreaseStock;

public class IncreaseStockCommandValidator : AbstractValidator<IncreaseStockCommand>
{
    public IncreaseStockCommandValidator()
    {
        RuleFor(x => x.StockItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
