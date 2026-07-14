namespace ECommerce.API.Controllers.Auth;

public record AuthResponse(string AccessToken, DateTime AccessTokenExpiresAt, string? RefreshToken);
