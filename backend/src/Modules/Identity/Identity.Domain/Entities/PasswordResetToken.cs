namespace Identity.Domain.Entities;

/// <summary>
/// Bridges two separate, far-apart HTTP requests (forgot-password issues it; reset-password
/// consumes it later, possibly minutes after, from a different process instance) — needs durable
/// storage for the same reason RefreshToken does. Only the hash is ever persisted; the raw token
/// only ever exists in the email link and the caller's reset-password request.
/// </summary>
public class PasswordResetToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public bool IsExpired => ExpiresAt < DateTime.UtcNow;
    public bool IsUsed => UsedAt is not null;

    private PasswordResetToken()
    {
    }

    public static PasswordResetToken Create(Guid userId, string tokenHash, DateTime expiresAt)
    {
        return new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkUsed() => UsedAt = DateTime.UtcNow;
}
