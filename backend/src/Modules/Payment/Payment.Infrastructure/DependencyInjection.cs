using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Abstractions;
using Payment.Application.Gateway;
using Payment.Application.Services;
using Payment.Contracts;
using Payment.Infrastructure.Gateway;
using Payment.Infrastructure.Integrations;
using Payment.Infrastructure.Options;
using Payment.Infrastructure.Persistence;

namespace Payment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPaymentDbContext>(sp => sp.GetRequiredService<PaymentDbContext>());

        services.Configure<IyzicoOptions>(configuration.GetSection(IyzicoOptions.SectionName));
        services.AddScoped<IIyzicoGateway, IyzicoGateway>();

        services.AddScoped<PaymentOperations>();
        services.AddScoped<IPaymentGateway, PaymentGatewayContractService>();

        return services;
    }
}
