using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions;
using Payment.Application.Gateway;
using Payment.Domain.Entities;
using Payment.Domain.Exceptions;

namespace Payment.Application.Services;

/// <summary>
/// Shared charge/refund logic, called in-process by Order via ISender (mirrors Order's own
/// OrderOperations). Only a successful charge is ever persisted — a declined attempt throws
/// PaymentDeclinedException without writing a row, since the order it would reference was never
/// persisted either (see Order.Application.Services.OrderOperations.PlaceOrderAsync).
/// </summary>
public class PaymentOperations(IPaymentDbContext dbContext, IIyzicoGateway gateway)
{
    public async Task<Guid> ChargeAsync(IyzicoChargeRequest request, CancellationToken cancellationToken)
    {
        var result = await gateway.ChargeAsync(request, cancellationToken);

        if (!result.IsSuccess)
            throw new PaymentDeclinedException(result.ErrorMessage ?? "Unknown reason.");

        var payment = PaymentTransaction.Create(
            request.OrderId,
            result.ProviderPaymentId!,
            request.PaidTotal,
            result.CardAssociation,
            result.CardFamily,
            result.CardLastFourDigits);

        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }

    public async Task RefundAsync(Guid orderId, string ip, CancellationToken cancellationToken)
    {
        var payment = await dbContext.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken)
            ?? throw new PaymentNotFoundException(orderId);

        if (payment.Status == Domain.Enums.PaymentStatus.Refunded)
            throw new PaymentAlreadyRefundedException(payment.Id);

        var result = await gateway.RefundAsync(new IyzicoRefundRequest(payment.ProviderPaymentId, ip), cancellationToken);

        if (!result.IsSuccess)
            throw new PaymentGatewayException(result.ErrorMessage ?? "Refund failed for an unknown reason.");

        payment.MarkRefunded();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
