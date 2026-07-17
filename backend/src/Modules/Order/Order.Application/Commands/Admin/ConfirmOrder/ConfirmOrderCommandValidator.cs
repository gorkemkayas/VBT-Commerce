using FluentValidation;

namespace Order.Application.Commands.Admin.ConfirmOrder;

public class ConfirmOrderCommandValidator : AbstractValidator<ConfirmOrderCommand>
{
    public ConfirmOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
