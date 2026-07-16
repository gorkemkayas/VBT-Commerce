using Customer.Application.Abstractions;
using Customer.Contracts;
using Customer.Infrastructure.Integrations;
using Customer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomerModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICustomerDbContext>(sp => sp.GetRequiredService<CustomerDbContext>());

        services.AddScoped<ICustomerDirectoryService, CustomerDirectoryService>();

        return services;
    }
}
