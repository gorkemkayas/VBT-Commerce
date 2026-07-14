using Identity.Application.Abstractions;
using Identity.Application.Commands.RefreshTokens;
using Identity.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Behaviors;

/// <summary>
/// Wraps RefreshTokenCommand's entire pipeline execution (including TransactionBehavior's
/// commit) in a per-user+platform lock. The lock must be released only after the transaction
/// commits — releasing it earlier (e.g. inside the handler, before TransactionBehavior commits)
/// would let a waiting concurrent request read the database before the winning request's
/// changes are actually visible.
/// </summary>
public class RefreshTokenConcurrencyBehavior(
    IIdentityDbContext dbContext,
    ITokenService tokenService,
    IRefreshTokenLockProvider lockProvider) : IPipelineBehavior<RefreshTokenCommand, AuthResult>
{
    public async Task<AuthResult> Handle(
        RefreshTokenCommand request,
        RequestHandlerDelegate<AuthResult> next,
        CancellationToken cancellationToken)
    {
        var incomingHash = tokenService.HashToken(request.RefreshToken);

        var tokenOwner = await dbContext.RefreshTokens
            .AsNoTracking()
            .Where(t => t.TokenHash == incomingHash)
            .Select(t => new { t.UserId, t.Platform })
            .FirstOrDefaultAsync(cancellationToken);

        if (tokenOwner is null)
            return await next(); // unknown token — let the handler produce the "invalid token" error

        var semaphore = lockProvider.GetLock(tokenOwner.UserId, tokenOwner.Platform);
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            return await next(); // runs TransactionBehavior + Handler; lock is held until commit finishes
        }
        finally
        {
            semaphore.Release();
        }
    }
}
