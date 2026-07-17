using MediatR;
using Payment.Application.Services;

namespace Payment.Application.Commands.Refunds.RefundOrderPayment;

public class RefundOrderPaymentCommandHandler(PaymentOperations paymentOperations) : IRequestHandler<RefundOrderPaymentCommand, Unit>
{
    public async Task<Unit> Handle(RefundOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        await paymentOperations.RefundAsync(request.OrderId, request.Ip, cancellationToken);
        return Unit.Value;
    }
}
