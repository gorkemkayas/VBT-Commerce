using FluentValidation;

namespace Inventory.Application.Commands.Reservations.ReserveStock;

public class ReserveStockCommandValidator : AbstractValidator<ReserveStockCommand>
{
    public ReserveStockCommandValidator()
    {
        RuleFor(x => x.ReferenceId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.SellableItemId).NotEmpty();
            item.RuleFor(i => i.SellableItemType).IsInEnum();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
