using Identity.Contracts;
using Notification.Application.Integrations;

namespace Notification.Infrastructure.Integrations;

/// <summary>
/// Adapts Notification's own IIdentityIntegrationService to Identity's actual contract
/// (IIdentityDirectoryService), so Notification.Application never references Identity directly.
/// </summary>
public class IdentityIntegrationService(IIdentityDirectoryService identityDirectoryService) : IIdentityIntegrationService
{
    public async Task<string?> GetUserEmailAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await identityDirectoryService.GetUserByIdAsync(userId, cancellationToken);
        return user?.Email;
    }
}
