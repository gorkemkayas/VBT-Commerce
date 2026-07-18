using Customer.Application.Abstractions;
using Customer.Domain.Entities;
using Identity.Application.Abstractions;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Seed;

/// <summary>
/// Development-only convenience seeder — creates a ready-to-use Admin and Customer account (the
/// Customer with a profile + default address already attached, so checkout can be tried immediately
/// without first walking through address creation) so a fresh clone doesn't need manual SQL to get a
/// usable Admin/Customer, the way earlier modules' manual testing repeatedly needed. Idempotent: each
/// run checks by email before inserting, so it's safe to call on every startup.
/// </summary>
public static class DevelopmentDataSeeder
{
    private const string AdminEmail = "admin@vbt-commerce.test";
    private const string AdminPassword = "Admin123!";
    private const string CustomerEmail = "customer@vbt-commerce.test";
    private const string CustomerPassword = "Customer123!";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var identityDbContext = scope.ServiceProvider.GetRequiredService<IIdentityDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var adminUser = await identityDbContext.Users.FirstOrDefaultAsync(u => u.Email == AdminEmail);
        if (adminUser is null)
        {
            adminUser = User.Register(AdminEmail, passwordHasher.Hash(AdminPassword), "Admin", "User");
            adminUser.PromoteToAdmin();
            identityDbContext.Users.Add(adminUser);
        }

        var customerUser = await identityDbContext.Users.FirstOrDefaultAsync(u => u.Email == CustomerEmail);
        if (customerUser is null)
        {
            customerUser = User.Register(CustomerEmail, passwordHasher.Hash(CustomerPassword), "Test", "Customer");
            identityDbContext.Users.Add(customerUser);
        }

        await identityDbContext.SaveChangesAsync(CancellationToken.None);

        var customerDbContext = scope.ServiceProvider.GetRequiredService<ICustomerDbContext>();

        var customerProfileExists = await customerDbContext.Customers.AnyAsync(c => c.UserId == customerUser.Id);
        if (!customerProfileExists)
        {
            var profile = CustomerProfile.Create(customerUser.Id, "5551234567", null);
            profile.AddAddress(
                "Home", "Test Customer", "5551234567", "Turkey", "Istanbul", "Kadikoy", "34000", "Test Sokak No:1", null, isDefault: true);

            customerDbContext.Customers.Add(profile);
            await customerDbContext.SaveChangesAsync(CancellationToken.None);
        }
    }
}
