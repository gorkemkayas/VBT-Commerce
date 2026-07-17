using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pricing.Application.Abstractions;
using Pricing.Application.Integrations;
using Pricing.Application.Services;
using Pricing.Contracts;
using Pricing.Infrastructure.Integrations;
using Pricing.Infrastructure.Persistence;

namespace Pricing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPricingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PricingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPricingDbContext>(sp => sp.GetRequiredService<PricingDbContext>());

        services.AddScoped<ICatalogIntegrationService, CatalogIntegrationService>();
        services.AddScoped<ICustomerIntegrationService, CustomerIntegrationService>();
        services.AddScoped<IPriceCatalogService, PriceContractService>();
        services.AddScoped<PriceCalculationService>();

        return services;
    }
}
