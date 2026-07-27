using Microsoft.EntityFrameworkCore;
using Order.Infrastructure.Persistence;

namespace Order.Application.Tests;

internal static class TestOrderDbContextFactory
{
    public static OrderDbContext Create()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new OrderDbContext(options);
    }
}
