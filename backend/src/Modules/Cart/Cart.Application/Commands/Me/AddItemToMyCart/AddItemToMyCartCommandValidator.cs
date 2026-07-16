using FluentValidation;

namespace Cart.Application.Commands.Me.AddItemToMyCart;

public class AddItemToMyCartCommandValidator : AbstractValidator<AddItemToMyCartCommand>
{
    public AddItemToMyCartCommandValidator()
    {
        RuleFor(x => x.SellableItemId).NotEmpty();
        RuleFor(x => x.SellableItemType).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
