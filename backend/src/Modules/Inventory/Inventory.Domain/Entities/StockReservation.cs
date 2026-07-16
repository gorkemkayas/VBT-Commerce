namespace Inventory.Domain.Entities;

/// <summary>
/// A temporary hold against a <see cref="StockItem"/>'s available quantity, grouped under a
/// caller-supplied <see cref="ReferenceId"/> (e.g. an Order id) so multiple line-item reservations
/// can be confirmed or released together as one unit.
/// </summary>
public class StockReservation
{
    public Guid Id { get; private set; }
    public Guid StockItemId { get; private set; }
    public Guid ReferenceId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsConfirmed { get; private set; }
    public bool IsReleased { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? ReleasedAt { get; private set; }

    private StockReservation()
    {
    }

    internal static StockReservation Create(Guid stockItemId, Guid referenceId, int quantity, DateTime expiresAt)
    {
        return new StockReservation
        {
            Id = Guid.NewGuid(),
            StockItemId = stockItemId,
            ReferenceId = referenceId,
            Quantity = quantity,
            ExpiresAt = expiresAt,
            IsConfirmed = false,
            IsReleased = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool IsActive(DateTime now) => !IsConfirmed && !IsReleased && ExpiresAt > now;

    internal void Confirm()
    {
        IsConfirmed = true;
        ConfirmedAt = DateTime.UtcNow;
    }

    internal void Release()
    {
        IsReleased = true;
        ReleasedAt = DateTime.UtcNow;
    }
}
