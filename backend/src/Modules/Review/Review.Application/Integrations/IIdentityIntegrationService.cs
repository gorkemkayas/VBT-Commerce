namespace Review.Application.Integrations;

/// <summary>
/// Review's own view of what it needs from the Identity module: a reviewer's display name, masked
/// before it ever reaches Review.Application so the raw name never leaks into a DTO by accident.
/// Implemented in Review.Infrastructure against Identity's IIdentityDirectoryService.
/// </summary>
public interface IIdentityIntegrationService
{
    Task<string?> GetMaskedDisplayNameAsync(Guid userId, CancellationToken cancellationToken);
}
