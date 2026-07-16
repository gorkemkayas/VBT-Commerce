using FluentValidation;

namespace Cart.Application.Commands.Me.RemoveMyCartItem;

public class RemoveMyCartItemCommandValidator : AbstractValidator<RemoveMyCartItemCommand>
{
    public RemoveMyCartItemCommandValidator()
    {
        RuleFor(x => x.CartItemId).NotEmpty();
    }
}
