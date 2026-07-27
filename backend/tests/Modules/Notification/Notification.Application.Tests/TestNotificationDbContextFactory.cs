using Microsoft.EntityFrameworkCore;
using Notification.Infrastructure.Persistence;

namespace Notification.Application.Tests;

internal static class TestNotificationDbContextFactory
{
    public static NotificationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new NotificationDbContext(options);
    }
}
