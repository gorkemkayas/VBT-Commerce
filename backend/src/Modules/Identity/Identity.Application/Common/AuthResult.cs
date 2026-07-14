namespace Identity.Application.Common;

public record AuthResult(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
