using Identity.Contracts;
using Review.Application.Integrations;

namespace Review.Infrastructure.Integrations;

/// <summary>
/// Adapts Review's own IIdentityIntegrationService to Identity's actual contract
/// (IIdentityDirectoryService), so Review.Application never references Identity directly.
/// </summary>
public class IdentityIntegrationService(IIdentityDirectoryService identityDirectoryService) : IIdentityIntegrationService
{
    public async Task<string?> GetMaskedDisplayNameAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await identityDirectoryService.GetUserByIdAsync(userId, cancellationToken);
        if (user is null)
            return null;

        // "Ahmet Y." — first name in full, last name reduced to its initial.
        return $"{user.FirstName} {user.LastName[0]}.";
    }
}
