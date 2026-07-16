using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;

namespace Inventory.Domain.Entities;

/// <summary>
/// Aggregate root tracking on-hand quantity and active reservations for a single sellable unit
/// (a Catalog Product for Simple products, or a ProductVariant for Variant products).
/// </summary>
public class StockItem
{
    private readonly List<StockReservation> _reservations = [];

    public Guid Id { get; private set; }
    public Guid SellableItemId { get; private set; }
    public InventoryItemType SellableItemType { get; private set; }
    public int QuantityOnHand { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<StockReservation> Reservations => _reservations.AsReadOnly();

    private StockItem()
    {
    }

    public static StockItem Create(Guid sellableItemId, InventoryItemType sellableItemType, int initialQuantity)
    {
        if (initialQuantity < 0)
            throw new InvalidStockQuantityException("Initial quantity cannot be negative.");

        return new StockItem
        {
            Id = Guid.NewGuid(),
            SellableItemId = sellableItemId,
            SellableItemType = sellableItemType,
            QuantityOnHand = initialQuantity,
            CreatedAt = DateTime.UtcNow
        };
    }

    public int AvailableQuantity(DateTime now)
        => QuantityOnHand - _reservations.Where(r => r.IsActive(now)).Sum(r => r.Quantity);

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new InvalidStockQuantityException("Quantity to add must be greater than zero.");

        QuantityOnHand += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecreaseQuantity(int amount)
    {
        if (amount <= 0)
            throw new InvalidStockQuantityException("Quantity to remove must be greater than zero.");

        if (QuantityOnHand - amount < 0)
            throw new InvalidStockQuantityException($"Cannot decrease stock item '{Id}' below zero.");

        QuantityOnHand -= amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public StockReservation Reserve(Guid referenceId, int quantity, DateTime expiresAt)
    {
        if (quantity <= 0)
            throw new InvalidStockQuantityException("Reservation quantity must be greater than zero.");

        var now = DateTime.UtcNow;
        var available = AvailableQuantity(now);

        if (available < quantity)
            throw new InsufficientStockException(Id, quantity, available);

        var reservation = StockReservation.Create(Id, referenceId, quantity, expiresAt);
        _reservations.Add(reservation);
        UpdatedAt = DateTime.UtcNow;

        return reservation;
    }

    public void ConfirmReservation(Guid reservationId)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId)
            ?? throw new StockReservationNotFoundException(reservationId);

        if (reservation.IsReleased)
            throw new ReservationAlreadyReleasedException(reservationId);

        if (reservation.IsConfirmed)
            throw new ReservationAlreadyConfirmedException(reservationId);

        if (QuantityOnHand - reservation.Quantity < 0)
            throw new InvalidStockQuantityException($"Cannot confirm reservation '{reservationId}': on-hand quantity would go below zero.");

        QuantityOnHand -= reservation.Quantity;
        reservation.Confirm();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReleaseReservation(Guid reservationId)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId)
            ?? throw new StockReservationNotFoundException(reservationId);

        if (reservation.IsConfirmed)
            throw new ReservationAlreadyConfirmedException(reservationId);

        if (reservation.IsReleased)
            throw new ReservationAlreadyReleasedException(reservationId);

        reservation.Release();
        UpdatedAt = DateTime.UtcNow;
    }
}
