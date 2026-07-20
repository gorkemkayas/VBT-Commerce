using Order.Application.Integrations;
using Payment.Contracts;

namespace Order.Infrastructure.Integrations;

/// <summary>
/// Adapts Order's own <see cref="IPaymentIntegrationService"/> to Payment's actual contract
/// (<see cref="IPaymentGateway"/>), so Order.Application never references Payment.Application
/// (or knows the provider is iyzico) directly.
/// </summary>
public class PaymentIntegrationService(IPaymentGateway paymentGateway) : IPaymentIntegrationService
{
    public Task<Guid> ChargeAsync(
        Guid orderId,
        decimal basketTotal,
        decimal paidTotal,
        PaymentCardInfo card,
        PaymentBuyerInfo buyer,
        PaymentAddressInfo address,
        IReadOnlyCollection<PaymentBasketItem> basketItems,
        CancellationToken cancellationToken)
        => paymentGateway.ChargeAsync(orderId, basketTotal, paidTotal, card, buyer, address, basketItems, cancellationToken);

    public Task RefundAsync(Guid orderId, string ip, CancellationToken cancellationToken)
        => paymentGateway.RefundAsync(orderId, ip, cancellationToken);
}
