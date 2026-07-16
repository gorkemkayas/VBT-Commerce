namespace Customer.Domain.Entities;

public class CustomerAddress
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Label { get; private set; } = null!;
    public string RecipientName { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string District { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;
    public string AddressLine1 { get; private set; } = null!;
    public string? AddressLine2 { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private CustomerAddress()
    {
    }

    internal static CustomerAddress Create(
        Guid customerId,
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
        return new CustomerAddress
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Label = label.Trim(),
            RecipientName = recipientName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Country = country.Trim(),
            City = city.Trim(),
            District = district.Trim(),
            PostalCode = postalCode.Trim(),
            AddressLine1 = addressLine1.Trim(),
            AddressLine2 = addressLine2,
            IsDefault = isDefault,
            CreatedAt = DateTime.UtcNow
        };
    }

    internal void Update(
        string label,
        string recipientName,
        string phoneNumber,
        string country,
        string city,
        string district,
        string postalCode,
        string addressLine1,
        string? addressLine2)
    {
        Label = label.Trim();
        RecipientName = recipientName.Trim();
        PhoneNumber = phoneNumber.Trim();
        Country = country.Trim();
        City = city.Trim();
        District = district.Trim();
        PostalCode = postalCode.Trim();
        AddressLine1 = addressLine1.Trim();
        AddressLine2 = addressLine2;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void SetDefault(bool isDefault)
    {
        IsDefault = isDefault;
        UpdatedAt = DateTime.UtcNow;
    }
}
