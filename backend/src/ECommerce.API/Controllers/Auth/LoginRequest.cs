using Identity.Domain.Enums;

namespace ECommerce.API.Controllers.Auth;

public record LoginRequest(string Email, string Password, ClientPlatform Platform, Guid? AnonymousId = null);
