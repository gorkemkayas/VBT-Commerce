using BuildingBlocks.Domain;

namespace Inventory.Domain.Exceptions;

public class StockReservationNotFoundException(Guid reservationId)
    : DomainException($"Stock reservation '{reservationId}' was not found.")
{
}
