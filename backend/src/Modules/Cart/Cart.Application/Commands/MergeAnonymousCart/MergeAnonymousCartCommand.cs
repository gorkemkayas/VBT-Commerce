using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Cart.Application.Commands.MergeAnonymousCart;

// No IRequireRole — this command isn't meant to be its own public endpoint. Its only legitimate
// caller is AuthController's login/register orchestration, immediately after a JWT has been minted
// for UserId (mirrors Inventory's Reserve/Confirm/Release: a command invoked in-process by another
// entry point rather than one the client calls directly, so it carries no role restriction of its
// own — the restriction lives at whichever entry point calls it).
public record MergeAnonymousCartCommand(Guid UserId, Guid AnonymousId) : ICommand<Unit>;
