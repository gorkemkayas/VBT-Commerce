using Identity.Application.Abstractions;
using Identity.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.Infrastructure.Services;

public class RefreshTokenLockProvider(IMemoryCache cache) : IRefreshTokenLockProvider
{
    public SemaphoreSlim GetLock(Guid userId, ClientPlatform platform)
    {
        var key = $"refreshlock:{userId}:{platform}";

        return cache.GetOrCreate(key, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            return new SemaphoreSlim(1, 1);
        })!;
    }
}
