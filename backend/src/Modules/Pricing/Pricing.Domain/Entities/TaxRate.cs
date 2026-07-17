using Pricing.Domain.Exceptions;

namespace Pricing.Domain.Entities;

/// <summary>
/// Single global tax rate (percentage) applied to every order — a singleton row, seeded once via
/// migration at <see cref="SingletonId"/> so a "no tax rate configured" state can never occur.
/// </summary>
public class TaxRate
{
    public static readonly Guid SingletonId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public Guid Id { get; private set; }
    public decimal Rate { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private TaxRate()
    {
    }

    public static TaxRate CreateDefault(decimal rate, DateTime? updatedAt = null)
    {
        Validate(rate);

        return new TaxRate
        {
            Id = SingletonId,
            Rate = rate,
            UpdatedAt = updatedAt ?? DateTime.UtcNow
        };
    }

    public void UpdateRate(decimal rate)
    {
        Validate(rate);

        Rate = rate;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(decimal rate)
    {
        if (rate is < 0 or > 100)
            throw new InvalidTaxRateException(rate);
    }
}
