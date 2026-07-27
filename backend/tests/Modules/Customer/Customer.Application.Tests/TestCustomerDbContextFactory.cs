using Customer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Customer.Application.Tests;

internal static class TestCustomerDbContextFactory
{
    public static CustomerDbContext Create()
    {
        var options = new DbContextOptionsBuilder<CustomerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CustomerDbContext(options);
    }
}
