using FluentValidation;

namespace Cart.Application.Commands.Anonymous.RemoveAnonymousCartItem;

public class RemoveAnonymousCartItemCommandValidator : AbstractValidator<RemoveAnonymousCartItemCommand>
{
    public RemoveAnonymousCartItemCommandValidator()
    {
        RuleFor(x => x.AnonymousId).NotEmpty();
        RuleFor(x => x.CartItemId).NotEmpty();
    }
}
