namespace Notification.Infrastructure.Options;

public class FrontendOptions
{
    public const string SectionName = "Frontend";

    public string PasswordResetUrl { get; set; } = null!;
}
