using Identity.Domain.Enums;

namespace ECommerce.API.Controllers.Auth.Requests;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, ClientPlatform Platform, Guid? AnonymousId = null);
