using Payment.Contracts;

namespace Order.Application.Integrations;

public interface IPaymentIntegrationService
{
    Task<Guid> ChargeAsync(
        Guid orderId,
        decimal basketTotal,
        decimal paidTotal,
        PaymentCardInfo card,
        PaymentBuyerInfo buyer,
        PaymentAddressInfo address,
        IReadOnlyCollection<PaymentBasketItem> basketItems,
        CancellationToken cancellationToken);

    Task RefundAsync(Guid orderId, string ip, CancellationToken cancellationToken);
}
