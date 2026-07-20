using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Cart.Application.Commands.ClearCartByUserId;

/// <summary>
/// No IRequireRole and no controller endpoint by design — meant to be called in-process by other
/// modules (Order, once checkout completes) rather than a customer's own HTTP request, which goes
/// through ClearMyCartCommand instead. Mirrors ClearAnonymousCartCommand's explicit-id shape so the
/// caller's identity does not need to flow through ICurrentUserService.
/// </summary>
public record ClearCartByUserIdCommand(Guid UserId) : ICommand<Unit>;
