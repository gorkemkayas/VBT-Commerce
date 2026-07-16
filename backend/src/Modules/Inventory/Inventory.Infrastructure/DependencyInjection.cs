using Inventory.Application.Abstractions;
using Inventory.Application.Integrations;
using Inventory.Contracts;
using Inventory.Infrastructure.Integrations;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoryModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IInventoryDbContext>(sp => sp.GetRequiredService<InventoryDbContext>());

        services.AddScoped<ICatalogIntegrationService, CatalogIntegrationService>();
        services.AddScoped<IInventoryService, InventoryContractService>();

        return services;
    }
}
