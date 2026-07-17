using Payment.Domain.Enums;

namespace Payment.Application.Common;

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    string ProviderPaymentId,
    decimal Amount,
    string? CardAssociation,
    string? CardFamily,
    string? CardLastFourDigits,
    PaymentStatus Status,
    DateTime CreatedAt,
    DateTime? RefundedAt);
