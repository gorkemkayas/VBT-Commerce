using Identity.Domain.Enums;

namespace ECommerce.API.Controllers.Auth.Requests;

public record LoginRequest(string Email, string Password, ClientPlatform Platform, Guid? AnonymousId = null);
