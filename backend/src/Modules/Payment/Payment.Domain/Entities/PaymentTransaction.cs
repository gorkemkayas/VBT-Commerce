using Payment.Domain.Enums;
using Payment.Domain.Exceptions;

namespace Payment.Domain.Entities;

/// <summary>
/// Records a successfully captured iyzico charge for an order. Named PaymentTransaction (not
/// Payment) to avoid colliding with the root Payment.* namespace tree — same precedent as
/// Order's CustomerOrder and Cart's ShoppingCart. Only successful charges are persisted: a
/// declined attempt never produces a row here (see PaymentOperations.ChargeAsync), so there is
/// no "Pending"/"Failed" status to model.
/// </summary>
public class PaymentTransaction
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string ProviderPaymentId { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public string? CardAssociation { get; private set; }
    public string? CardFamily { get; private set; }
    public string? CardLastFourDigits { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }

    private PaymentTransaction()
    {
    }

    public static PaymentTransaction Create(
        Guid orderId,
        string providerPaymentId,
        decimal amount,
        string? cardAssociation,
        string? cardFamily,
        string? cardLastFourDigits)
    {
        return new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProviderPaymentId = providerPaymentId,
            Amount = amount,
            CardAssociation = cardAssociation,
            CardFamily = cardFamily,
            CardLastFourDigits = cardLastFourDigits,
            Status = PaymentStatus.Succeeded,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkRefunded()
    {
        if (Status != PaymentStatus.Succeeded)
            throw new PaymentAlreadyRefundedException(Id);

        Status = PaymentStatus.Refunded;
        RefundedAt = DateTime.UtcNow;
    }
}
