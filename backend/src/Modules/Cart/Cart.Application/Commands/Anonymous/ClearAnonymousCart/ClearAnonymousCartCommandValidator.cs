using FluentValidation;

namespace Cart.Application.Commands.Anonymous.ClearAnonymousCart;

public class ClearAnonymousCartCommandValidator : AbstractValidator<ClearAnonymousCartCommand>
{
    public ClearAnonymousCartCommandValidator()
    {
        RuleFor(x => x.AnonymousId).NotEmpty();
    }
}
