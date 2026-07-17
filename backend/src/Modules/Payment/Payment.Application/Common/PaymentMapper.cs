using Payment.Domain.Entities;

namespace Payment.Application.Common;

public static class PaymentMapper
{
    public static PaymentDto ToDto(PaymentTransaction payment) => new(
        payment.Id,
        payment.OrderId,
        payment.ProviderPaymentId,
        payment.Amount,
        payment.CardAssociation,
        payment.CardFamily,
        payment.CardLastFourDigits,
        payment.Status,
        payment.CreatedAt,
        payment.RefundedAt);
}
