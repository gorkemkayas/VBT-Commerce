namespace Notification.Application.Integrations;

/// <summary>
/// Notification's own view of what it needs from the Identity module: a registered user's email,
/// looked up by UserId rather than read off ICurrentUserService — this handler runs from a
/// background outbox processor with no HTTP/JWT request context to read a "current user" from.
/// Implemented in Notification.Infrastructure against Identity's IIdentityDirectoryService.
/// </summary>
public interface IIdentityIntegrationService
{
    Task<string?> GetUserEmailAsync(Guid userId, CancellationToken cancellationToken);
}
