using Microsoft.EntityFrameworkCore;
using Shipping.Infrastructure.Persistence;

namespace Shipping.Application.Tests;

internal static class TestShippingDbContextFactory
{
    public static ShippingDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ShippingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ShippingDbContext(options);
    }
}
