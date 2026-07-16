using BuildingBlocks.Domain;

namespace Inventory.Domain.Exceptions;

public class NoActiveReservationsForReferenceException(Guid referenceId)
    : DomainException($"Reference '{referenceId}' has reservations, but none of them are currently active (already confirmed or released).")
{
}
