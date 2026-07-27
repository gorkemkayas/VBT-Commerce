using Microsoft.EntityFrameworkCore;
using Pricing.Infrastructure.Persistence;

namespace Pricing.Application.Tests;

internal static class TestPricingDbContextFactory
{
    public static PricingDbContext Create()
    {
        var options = new DbContextOptionsBuilder<PricingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PricingDbContext(options);
    }
}
