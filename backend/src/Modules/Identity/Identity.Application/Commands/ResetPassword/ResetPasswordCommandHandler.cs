using Identity.Application.Abstractions;
using Identity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IIdentityDbContext dbContext, ITokenService tokenService, IPasswordHasher passwordHasher)
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = tokenService.HashToken(request.Token);

        var resetToken = await dbContext.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (resetToken is null || resetToken.IsUsed || resetToken.IsExpired)
            throw new InvalidOrExpiredResetTokenException();

        var user = await dbContext.Users.FirstAsync(u => u.Id == resetToken.UserId, cancellationToken);

        user.SetPasswordHash(passwordHasher.Hash(request.NewPassword));
        resetToken.MarkUsed();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
