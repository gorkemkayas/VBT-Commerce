using Microsoft.EntityFrameworkCore;
using Review.Infrastructure.Persistence;

namespace Review.Application.Tests;

internal static class TestReviewDbContextFactory
{
    public static ReviewDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ReviewDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ReviewDbContext(options);
    }
}
