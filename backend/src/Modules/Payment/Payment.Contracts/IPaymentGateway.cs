namespace Payment.Contracts;

/// <summary>
/// The Payment module's outbound contract (architecture.md §3). Other modules (chiefly Order,
/// during checkout) depend on this instead of Payment's internal Application commands, and never
/// learn that the underlying provider is iyzico.
/// </summary>
public interface IPaymentGateway
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
