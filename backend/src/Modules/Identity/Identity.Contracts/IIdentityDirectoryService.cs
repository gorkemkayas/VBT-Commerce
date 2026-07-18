namespace Identity.Contracts;

/// <summary>
/// Read-only contract exposed by the Identity module to other modules (e.g. Notification, which
/// needs a registered user's email from a background service with no HTTP/JWT request context to
/// read it from — unlike Order's checkout handlers, which read it off ICurrentUserService.Email
/// within the request that's already authenticated).
/// </summary>
public interface IIdentityDirectoryService
{
    Task<UserSummaryDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
}
