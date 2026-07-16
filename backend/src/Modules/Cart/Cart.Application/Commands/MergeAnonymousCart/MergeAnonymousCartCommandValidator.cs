using FluentValidation;

namespace Cart.Application.Commands.MergeAnonymousCart;

public class MergeAnonymousCartCommandValidator : AbstractValidator<MergeAnonymousCartCommand>
{
    public MergeAnonymousCartCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AnonymousId).NotEmpty();
    }
}
