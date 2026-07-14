using Identity.Domain.Enums;

namespace Identity.Application.Abstractions;

public interface IRefreshTokenLockProvider
{
    SemaphoreSlim GetLock(Guid userId, ClientPlatform platform);
}
