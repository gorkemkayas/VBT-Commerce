using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shipping.Application.Abstractions;
using Shipping.Contracts;
using Shipping.Infrastructure.Integrations;
using Shipping.Infrastructure.Persistence;

namespace Shipping.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddShippingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ShippingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IShippingDbContext>(sp => sp.GetRequiredService<ShippingDbContext>());

        services.AddScoped<IShippingCatalogService, ShippingContractService>();

        return services;
    }
}
