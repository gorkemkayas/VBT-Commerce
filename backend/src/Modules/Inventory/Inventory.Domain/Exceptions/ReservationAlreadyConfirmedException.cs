using BuildingBlocks.Domain;

namespace Inventory.Domain.Exceptions;

public class ReservationAlreadyConfirmedException(Guid reservationId)
    : DomainException($"Reservation '{reservationId}' is already confirmed and cannot be modified.")
{
}
