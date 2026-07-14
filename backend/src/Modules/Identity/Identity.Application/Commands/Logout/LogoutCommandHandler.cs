using Identity.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Commands.Logout;

public class LogoutCommandHandler(IIdentityDbContext dbContext, ITokenService tokenService) : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var hash = tokenService.HashToken(request.RefreshToken);

        var tokenEntity = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        if (tokenEntity is not null && !tokenEntity.IsRevoked)
        {
            tokenEntity.Revoke();
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
