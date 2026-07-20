using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.Application.Abstractions;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Identity.Infrastructure.Services;

public class TokenService(IOptions<JwtOptions> jwtOptions, IOptions<PasswordResetOptions> passwordResetOptions) : ITokenService
{
    private readonly JwtOptions _options = jwtOptions.Value;
    private readonly PasswordResetOptions _passwordResetOptions = passwordResetOptions.Value;

    public (string Token, DateTime ExpiresAt) GenerateAccessToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public (string RawToken, string TokenHash, DateTime ExpiresAt) GenerateRefreshToken(ClientPlatform platform)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var refreshTokenDays = platform == ClientPlatform.Mobile
            ? _options.RefreshTokenDaysMobile
            : _options.RefreshTokenDaysWeb;

        var expiresAt = DateTime.UtcNow.AddDays(refreshTokenDays);

        return (rawToken, HashToken(rawToken), expiresAt);
    }

    public (string RawToken, string TokenHash, DateTime ExpiresAt) GeneratePasswordResetToken()
    {
        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var expiresAt = DateTime.UtcNow.AddMinutes(_passwordResetOptions.TokenExpiryMinutes);

        return (rawToken, HashToken(rawToken), expiresAt);
    }

    public string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }
}
