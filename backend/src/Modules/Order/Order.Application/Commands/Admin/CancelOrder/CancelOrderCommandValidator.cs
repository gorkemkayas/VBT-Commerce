using FluentValidation;

namespace Order.Application.Commands.Admin.CancelOrder;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
