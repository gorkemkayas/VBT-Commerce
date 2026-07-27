using Microsoft.EntityFrameworkCore;
using Payment.Infrastructure.Persistence;

namespace Payment.Application.Tests;

internal static class TestPaymentDbContextFactory
{
    public static PaymentDbContext Create()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PaymentDbContext(options);
    }
}
