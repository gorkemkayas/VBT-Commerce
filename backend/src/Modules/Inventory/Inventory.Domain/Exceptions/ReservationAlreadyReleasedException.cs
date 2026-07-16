using BuildingBlocks.Domain;

namespace Inventory.Domain.Exceptions;

public class ReservationAlreadyReleasedException(Guid reservationId)
    : DomainException($"Reservation '{reservationId}' has already been released.")
{
}
