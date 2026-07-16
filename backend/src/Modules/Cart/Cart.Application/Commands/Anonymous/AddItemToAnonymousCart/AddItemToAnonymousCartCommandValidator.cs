using FluentValidation;

namespace Cart.Application.Commands.Anonymous.AddItemToAnonymousCart;

public class AddItemToAnonymousCartCommandValidator : AbstractValidator<AddItemToAnonymousCartCommand>
{
    public AddItemToAnonymousCartCommandValidator()
    {
        RuleFor(x => x.AnonymousId).NotEmpty();
        RuleFor(x => x.SellableItemId).NotEmpty();
        RuleFor(x => x.SellableItemType).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
