using FluentValidation;

namespace Order.Application.Commands.Me.CancelMyOrder;

public class CancelMyOrderCommandValidator : AbstractValidator<CancelMyOrderCommand>
{
    public CancelMyOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
