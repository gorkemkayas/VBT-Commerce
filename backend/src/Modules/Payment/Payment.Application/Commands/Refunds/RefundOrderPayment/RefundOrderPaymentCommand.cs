using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Payment.Application.Commands.Refunds.RefundOrderPayment;

/// <summary>
/// No IRequireRole and no controller by design — called in-process via ISender from Order's
/// OrderOperations.CancelAsync when cancelling an already-Confirmed order (mirrors
/// ChargeOrderPaymentCommand). Ip is the requester's IP (Admin or the customer cancelling their
/// own order), forwarded to iyzico's Cancel API.
/// </summary>
public record RefundOrderPaymentCommand(Guid OrderId, string Ip) : ICommand<Unit>;
