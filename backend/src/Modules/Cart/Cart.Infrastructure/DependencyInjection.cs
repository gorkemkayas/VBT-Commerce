using Cart.Application.Abstractions;
using Cart.Application.Integrations;
using Cart.Application.Services;
using Cart.Infrastructure.Integrations;
using Cart.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cart.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCartModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CartDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICartDbContext>(sp => sp.GetRequiredService<CartDbContext>());

        services.AddScoped<ICatalogIntegrationService, CatalogIntegrationService>();
        services.AddScoped<IInventoryIntegrationService, InventoryIntegrationService>();
        services.AddScoped<CartOperations>();

        return services;
    }
}
