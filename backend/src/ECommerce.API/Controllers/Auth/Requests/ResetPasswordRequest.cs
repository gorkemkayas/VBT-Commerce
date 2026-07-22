namespace ECommerce.API.Controllers.Auth.Requests;

public record ResetPasswordRequest(string Token, string NewPassword);
