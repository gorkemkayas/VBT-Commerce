using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Tests;

internal static class TestCatalogDbContextFactory
{
    public static CatalogDbContext Create()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CatalogDbContext(options);
    }
}
