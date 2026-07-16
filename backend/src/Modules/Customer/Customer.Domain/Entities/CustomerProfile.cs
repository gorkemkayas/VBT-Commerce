using Customer.Domain.Exceptions;

namespace Customer.Domain.Entities;

/// <summary>
/// Aggregate root extending Identity's User with customer-specific profile data and addresses.
/// Does not duplicate Email/Name — those stay in Identity; this only adds what Identity doesn't have.
/// </summary>
public class CustomerProfile
{
    private readonly List<CustomerAddress> _addresses = [];

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string? PhoneNumber { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

    private CustomerProfile()
    {
    }

    public static CustomerProfile Create(Guid userId, string? phoneNumber, DateOnly? dateOfBirth)
    {
        return new CustomerProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PhoneNumber = phoneNumber,
            DateOfBirth = dateOfBirth,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string? phoneNumber, DateOnly? dateOfBirth)
    {
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        UpdatedAt = DateTime.UtcNow;
    }

    public CustomerAddress AddAddress(
        string label,
        string recipientName,
        string phoneNumber,
        string country,
        string city,
        string district,
        string postalCode,
        string addressLine1,
        string? addressLine2,
        bool isDefault)
    {
        // The first address a customer adds is always the default, regardless of what was requested.
        var shouldBeDefault = isDefault || _addresses.Count == 0;

        if (shouldBeDefault)
            UnsetExistingDefaultAddress();

        var address = CustomerAddress.Create(
            Id, label, recipientName, phoneNumber, country, city, district, postalCode, addressLine1, addressLine2, shouldBeDefault);

        _addresses.Add(address);
        UpdatedAt = DateTime.UtcNow;

        return address;
    }

    public void UpdateAddress(
        Guid addressId,
        string label,
        string recipientName,
        string phoneNumber,
        string country,
        string city,
        string district,
        string postalCode,
        string addressLine1,
        string? addressLine2,
        bool isDefault)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId)
            ?? throw new CustomerAddressNotFoundException(addressId);

        // A lone address can't lose its default status — there must always be exactly one default
        // once a customer has at least one address.
        var shouldBeDefault = isDefault || _addresses.Count == 1;

        address.Update(label, recipientName, phoneNumber, country, city, district, postalCode, addressLine1, addressLine2);

        if (shouldBeDefault && !address.IsDefault)
        {
            UnsetExistingDefaultAddress();
            address.SetDefault(true);
        }
        else if (!shouldBeDefault && address.IsDefault)
        {
            address.SetDefault(false);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId)
            ?? throw new CustomerAddressNotFoundException(addressId);

        _addresses.Remove(address);

        // Promote another address to default so a customer with remaining addresses always has one.
        if (address.IsDefault)
        {
            var nextDefault = _addresses.FirstOrDefault();
            nextDefault?.SetDefault(true);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDefaultAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId)
            ?? throw new CustomerAddressNotFoundException(addressId);

        if (address.IsDefault)
            return;

        UnsetExistingDefaultAddress();
        address.SetDefault(true);
        UpdatedAt = DateTime.UtcNow;
    }

    private void UnsetExistingDefaultAddress()
    {
        foreach (var existingDefault in _addresses.Where(a => a.IsDefault))
            existingDefault.SetDefault(false);
    }
}
