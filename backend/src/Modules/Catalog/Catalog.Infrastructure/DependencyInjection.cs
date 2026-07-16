using Catalog.Application.Abstractions;
using Catalog.Contracts;
using Catalog.Infrastructure.Integrations;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICatalogDbContext>(sp => sp.GetRequiredService<CatalogDbContext>());

        services.AddScoped<IProductCatalogService, ProductCatalogService>();

        return services;
    }
}
