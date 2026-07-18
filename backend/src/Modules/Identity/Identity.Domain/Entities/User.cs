using Identity.Domain.Enums;

namespace Identity.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string? PasswordHash { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private User()
    {
    }

    public static User Register(string email, string passwordHash, string firstName, string lastName)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Role = UserRole.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>Used only by the Development seeder — there is no admin-facing "promote user" flow.</summary>
    public void PromoteToAdmin() => Role = UserRole.Admin;
}
