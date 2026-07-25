using Cart.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence;
using Customer.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Notification.Infrastructure.Persistence;
using Order.Infrastructure.Persistence;
using Payment.Infrastructure.Persistence;
using Pricing.Infrastructure.Persistence;
using Review.Infrastructure.Persistence;
using Shipping.Infrastructure.Persistence;

namespace ECommerce.API.Extensions;

public static class DatabaseMigrationExtensions
{
    // Applies each module's pending EF Core migrations on boot, so a deploy no longer requires
    // SSHing in and running `dotnet ef database update` per module by hand. Safe to call on every
    // startup — MigrateAsync no-ops when a module has nothing pending. Only safe as long as a
    // single API instance runs at a time; concurrent instances migrating simultaneously could race.
    public static async Task MigrateModuleDatabasesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        await services.GetRequiredService<CartDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<CatalogDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<CustomerDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<IdentityDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<InventoryDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<NotificationDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<OrderDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<PaymentDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<PricingDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<ReviewDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<ShippingDbContext>().Database.MigrateAsync();
    }
}
