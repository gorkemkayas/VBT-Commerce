using Shipping.Domain.Exceptions;

namespace Shipping.Domain.Entities;

/// <summary>
/// A manually managed shipping company with a single flat fee (project-overview.md §4: MVP shipping
/// is manual/static, no carrier API integration — the company is selected at checkout for information only).
/// </summary>
public class ShippingCompany
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal Fee { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private ShippingCompany()
    {
    }

    public static ShippingCompany Create(string name, decimal fee)
    {
        if (fee <= 0)
            throw new InvalidShippingFeeException(fee);

        return new ShippingCompany
        {
            Id = Guid.NewGuid(),
            Name = name,
            Fee = fee,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, decimal fee)
    {
        if (fee <= 0)
            throw new InvalidShippingFeeException(fee);

        Name = name;
        Fee = fee;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
