using Cart.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cart.Application.Tests;

internal static class TestCartDbContextFactory
{
    public static CartDbContext Create()
    {
        var options = new DbContextOptionsBuilder<CartDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CartDbContext(options);
    }
}
