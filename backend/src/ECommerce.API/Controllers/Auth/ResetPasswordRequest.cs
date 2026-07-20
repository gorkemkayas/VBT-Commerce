namespace ECommerce.API.Controllers.Auth;

public record ResetPasswordRequest(string Token, string NewPassword);
