using FluentValidation;

namespace Payment.Application.Commands.Refunds.RefundOrderPayment;

public class RefundOrderPaymentCommandValidator : AbstractValidator<RefundOrderPaymentCommand>
{
    public RefundOrderPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Ip).NotEmpty();
    }
}
