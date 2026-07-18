using Identity.Application.Abstractions;
using Identity.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Integrations;

public class IdentityDirectoryService(IIdentityDbContext dbContext) : IIdentityDirectoryService
{
    public async Task<UserSummaryDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user is null ? null : new UserSummaryDto(user.Id, user.Email, user.FirstName, user.LastName);
    }
}
