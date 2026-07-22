namespace ECommerce.API.Controllers.Auth.Responses;

public record AuthResponse(string AccessToken, DateTime AccessTokenExpiresAt, string? RefreshToken);
