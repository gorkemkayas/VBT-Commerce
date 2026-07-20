using Identity.Application.Abstractions;
using Identity.Contracts.Events;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Commands.ForgotPassword;

/// <summary>
/// Always succeeds regardless of whether the email matches a user — responding differently for a
/// known vs. unknown email would let a caller enumerate registered addresses. The reset token and
/// email are only ever created/sent when a match is actually found.
/// </summary>
public class ForgotPasswordCommandHandler(IIdentityDbContext dbContext, ITokenService tokenService, IPublisher publisher)
    : IRequestHandler<ForgotPasswordCommand, Unit>
{
    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (user is null)
            return Unit.Value;

        var (rawToken, tokenHash, expiresAt) = tokenService.GeneratePasswordResetToken();
        dbContext.PasswordResetTokens.Add(PasswordResetToken.Create(user.Id, tokenHash, expiresAt));
        await dbContext.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new PasswordResetRequestedEvent(user.Id, user.Email, rawToken), cancellationToken);

        return Unit.Value;
    }
}
