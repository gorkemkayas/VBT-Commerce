using Microsoft.Extensions.Options;
using Notification.Application.Links;
using Notification.Infrastructure.Options;

namespace Notification.Infrastructure.Links;

public class PasswordResetLinkBuilder(IOptions<FrontendOptions> options) : IPasswordResetLinkBuilder
{
    public string Build(string rawToken) => $"{options.Value.PasswordResetUrl}?token={Uri.EscapeDataString(rawToken)}";
}
