using MediatR;

namespace Identity.Contracts.Events;

/// <summary>
/// Published (directly, synchronously — no Outbox) by ForgotPasswordCommandHandler once a matching
/// user is found and a reset token has been persisted. Unlike Order's OrderConfirmedEvent, this
/// doesn't need Outbox durability: a forgot-password click isn't on a latency-critical path the way
/// checkout is, so paying the SMTP round-trip inline is an acceptable, simpler tradeoff.
/// </summary>
public record PasswordResetRequestedEvent(Guid UserId, string Email, string RawToken) : INotification;
