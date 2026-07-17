using MediatR;
using Payment.Application.Gateway;
using Payment.Application.Services;

namespace Payment.Application.Commands.Charges.ChargeOrderPayment;

public class ChargeOrderPaymentCommandHandler(PaymentOperations paymentOperations) : IRequestHandler<ChargeOrderPaymentCommand, Guid>
{
    public Task<Guid> Handle(ChargeOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        var chargeRequest = new IyzicoChargeRequest(
            request.OrderId, request.BasketTotal, request.PaidTotal, request.Card, request.Buyer, request.Address, request.BasketItems);

        return paymentOperations.ChargeAsync(chargeRequest, cancellationToken);
    }
}
