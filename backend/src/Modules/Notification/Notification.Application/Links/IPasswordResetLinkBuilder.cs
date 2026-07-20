namespace Notification.Application.Links;

/// <summary>
/// Builds the URL embedded in a password-reset email. Implemented in Notification.Infrastructure
/// against a configured base URL — currently a placeholder pointing at this API itself since no
/// frontend exists yet; only that config value needs to change once the real frontend route does.
/// </summary>
public interface IPasswordResetLinkBuilder
{
    string Build(string rawToken);
}
