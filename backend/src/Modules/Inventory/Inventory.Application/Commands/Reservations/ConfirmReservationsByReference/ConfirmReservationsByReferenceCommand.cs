using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Inventory.Application.Commands.Reservations.ConfirmReservationsByReference;

/// <summary>
/// Confirms every active reservation grouped under <paramref name="ReferenceId"/> (e.g. once an
/// Order's payment succeeds). No <c>IRequireRole</c> — called in-process by other modules.
/// </summary>
public record ConfirmReservationsByReferenceCommand(Guid ReferenceId) : ICommand<Unit>;
