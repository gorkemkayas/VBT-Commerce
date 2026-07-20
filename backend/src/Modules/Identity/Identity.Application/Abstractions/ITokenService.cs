using Identity.Domain.Entities;
using Identity.Domain.Enums;

namespace Identity.Application.Abstractions;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);

    (string RawToken, string TokenHash, DateTime ExpiresAt) GenerateRefreshToken(ClientPlatform platform);

    (string RawToken, string TokenHash, DateTime ExpiresAt) GeneratePasswordResetToken();

    string HashToken(string rawToken);
}
