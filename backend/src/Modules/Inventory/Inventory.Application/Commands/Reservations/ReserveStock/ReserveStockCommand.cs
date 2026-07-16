using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Inventory.Application.Commands.Reservations.ReserveStock;

/// <summary>
/// Reserves stock for one or more line items under a single caller-supplied <paramref name="ReferenceId"/>
/// (e.g. an Order id) so they can later be confirmed or released together.
/// Intentionally carries no <c>IRequireRole</c> — this is meant to be called in-process by other
/// modules (Order) during a customer's own checkout request, not gated behind an Admin role.
/// </summary>
public record ReserveStockCommand(Guid ReferenceId, IReadOnlyCollection<ReserveStockLineItem> Items) : ICommand<Unit>;
