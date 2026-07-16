using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Inventory.Application.Commands.Reservations.ReleaseReservationsByReference;

/// <summary>
/// Releases every active reservation grouped under <paramref name="ReferenceId"/> (compensating
/// action when an Order fails to complete). No <c>IRequireRole</c> — called in-process by other modules.
/// </summary>
public record ReleaseReservationsByReferenceCommand(Guid ReferenceId) : ICommand<Unit>;
