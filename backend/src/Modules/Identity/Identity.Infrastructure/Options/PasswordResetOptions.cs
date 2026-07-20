namespace Identity.Infrastructure.Options;

public class PasswordResetOptions
{
    public const string SectionName = "PasswordReset";

    public int TokenExpiryMinutes { get; set; } = 30;
}
