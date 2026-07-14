using Identity.Domain.Enums;

namespace Identity.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public ClientPlatform Platform { get; private set; }
    public Guid FamilyId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private RefreshToken()
    {
    }

    public static RefreshToken Create(Guid userId, ClientPlatform platform, Guid familyId, string tokenHash, DateTime expiresAt)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Platform = platform,
            FamilyId = familyId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool IsExpired => ExpiresAt < DateTime.UtcNow;

    public void Revoke(Guid? replacedByTokenId = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        ReplacedByTokenId = replacedByTokenId;
    }
}
