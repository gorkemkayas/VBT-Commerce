using FluentValidation;

namespace Cart.Application.Commands.Anonymous.UpdateAnonymousCartItemQuantity;

public class UpdateAnonymousCartItemQuantityCommandValidator : AbstractValidator<UpdateAnonymousCartItemQuantityCommand>
{
    public UpdateAnonymousCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.AnonymousId).NotEmpty();
        RuleFor(x => x.CartItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
