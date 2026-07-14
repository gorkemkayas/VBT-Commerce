using Identity.Domain.Enums;

namespace ECommerce.API.Controllers.Auth;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, ClientPlatform Platform);
