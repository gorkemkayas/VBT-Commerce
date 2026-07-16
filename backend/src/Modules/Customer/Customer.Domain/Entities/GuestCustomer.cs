namespace Customer.Domain.Entities;

/// <summary>
/// Lightweight, standalone record for guest checkout — not a subtype of <see cref="CustomerProfile"/>.
/// Carries only identity info; shipping/billing address for a guest order is captured on the Order
/// itself at checkout time rather than stored here for reuse.
/// </summary>
public class GuestCustomer
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private GuestCustomer()
    {
    }

    public static GuestCustomer Create(string firstName, string lastName, string email, string phoneNumber)
    {
        return new GuestCustomer
        {
            Id = Guid.NewGuid(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PhoneNumber = phoneNumber.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
