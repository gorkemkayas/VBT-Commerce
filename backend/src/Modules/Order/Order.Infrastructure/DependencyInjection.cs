using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Abstractions;
using Order.Application.Integrations;
using Order.Application.Services;
using Order.Contracts;
using Order.Infrastructure.Integrations;
using Order.Infrastructure.Persistence;

namespace Order.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IOrderDbContext>(sp => sp.GetRequiredService<OrderDbContext>());

        services.AddScoped<ICartIntegrationService, CartIntegrationService>();
        services.AddScoped<ICustomerIntegrationService, CustomerIntegrationService>();
        services.AddScoped<IShippingIntegrationService, ShippingIntegrationService>();
        services.AddScoped<IOrderPurchaseVerifier, OrderPurchaseVerifierService>();
        services.AddScoped<OrderOperations>();

        return services;
    }
}
