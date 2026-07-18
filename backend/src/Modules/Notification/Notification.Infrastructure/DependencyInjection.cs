using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Abstractions;
using Notification.Application.Email;
using Notification.Application.Integrations;
using Notification.Infrastructure.Email;
using Notification.Infrastructure.Integrations;
using Notification.Infrastructure.Options;
using Notification.Infrastructure.Persistence;

namespace Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<INotificationDbContext>(sp => sp.GetRequiredService<NotificationDbContext>());

        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.AddScoped<IEmailSender, MailKitEmailSender>();

        services.AddScoped<ICustomerIntegrationService, CustomerIntegrationService>();
        services.AddScoped<IIdentityIntegrationService, IdentityIntegrationService>();

        return services;
    }
}
