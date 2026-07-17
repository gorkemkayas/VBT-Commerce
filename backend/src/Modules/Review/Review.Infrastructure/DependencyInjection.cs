using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Review.Application.Abstractions;
using Review.Application.Integrations;
using Review.Infrastructure.Integrations;
using Review.Infrastructure.Persistence;

namespace Review.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReviewModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReviewDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IReviewDbContext>(sp => sp.GetRequiredService<ReviewDbContext>());

        services.AddScoped<ICatalogIntegrationService, CatalogIntegrationService>();
        services.AddScoped<IOrderIntegrationService, OrderIntegrationService>();

        return services;
    }
}
