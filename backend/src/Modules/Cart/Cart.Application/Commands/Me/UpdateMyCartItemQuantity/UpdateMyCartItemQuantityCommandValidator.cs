using FluentValidation;

namespace Cart.Application.Commands.Me.UpdateMyCartItemQuantity;

public class UpdateMyCartItemQuantityCommandValidator : AbstractValidator<UpdateMyCartItemQuantityCommand>
{
    public UpdateMyCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.CartItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
