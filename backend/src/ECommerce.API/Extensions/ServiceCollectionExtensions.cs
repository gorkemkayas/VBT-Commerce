using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.Security;
using Cart.Application.Commands.Anonymous.AddItemToAnonymousCart;
using Cart.Infrastructure;
using Catalog.Application.Commands.Categories.CreateCategory;
using Catalog.Infrastructure;
using Customer.Application.Commands.GuestCustomers.CreateGuestCustomer;
using Customer.Infrastructure;
using ECommerce.API.Middleware;
using ECommerce.API.Security;
using FluentValidation;
using Identity.Application.Commands.RefreshTokens;
using Identity.Application.Commands.Register;
using Identity.Application.Common;
using Identity.Infrastructure;
using Inventory.Application.Commands.StockItems.CreateStockItem;
using Inventory.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using Notification.Application.Queries.GetNotificationLogsList;
using Notification.Infrastructure;
using Order.Application.Commands.Checkout.PlaceMyOrder;
using Order.Infrastructure;
using Payment.Application.Commands.Charges.ChargeOrderPayment;
using Payment.Infrastructure;
using Pricing.Application.Commands.Prices.CreatePrice;
using Pricing.Infrastructure;
using Review.Application.Commands.Me.CreateMyReview;
using Review.Infrastructure;
using Shipping.Application.Commands.ShippingCompanies.CreateShippingCompany;
using Shipping.Infrastructure;

namespace ECommerce.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // Team dev/staging server sits behind a reverse proxy that terminates TLS — without this, Kestrel
        // sees every request as plain HTTP, so Request.IsHttps (used for the refresh-token cookie's Secure
        // flag below) would always read false even when the client connected over HTTPS.
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        // Frontend/mobile teammates call this API from their own origin(s) — configured, not hardcoded, so
        // a new origin (e.g. once the frontend is actually deployed) can be added via config without a
        // redeploy. AllowCredentials is required because the Web refresh-token flow relies on a cookie.
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options =>
        {
            options.AddPolicy("TeamDev", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityModule(configuration);
        services.AddCatalogModule(configuration);
        services.AddInventoryModule(configuration);
        services.AddCustomerModule(configuration);
        services.AddCartModule(configuration);
        services.AddPricingModule(configuration);
        services.AddShippingModule(configuration);
        services.AddPaymentModule(configuration);
        services.AddOrderModule(configuration);
        services.AddReviewModule(configuration);
        services.AddNotificationModule(configuration);

        return services;
    }

    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        // CQRS: MediatR + FluentValidation + Pipeline Behaviors (module assemblies register their own handlers/validators)
        var moduleAssemblies = new[]
        {
            typeof(RegisterCommand).Assembly,
            typeof(CreateCategoryCommand).Assembly,
            typeof(CreateStockItemCommand).Assembly,
            typeof(CreateGuestCustomerCommand).Assembly,
            typeof(AddItemToAnonymousCartCommand).Assembly,
            typeof(CreatePriceCommand).Assembly,
            typeof(CreateShippingCompanyCommand).Assembly,
            typeof(ChargeOrderPaymentCommand).Assembly,
            typeof(PlaceMyOrderCommand).Assembly,
            typeof(CreateMyReviewCommand).Assembly,
            typeof(GetNotificationLogsListQuery).Assembly
        };

        // Registered before the open behaviors below so it becomes the outermost wrapper — its lock
        // must stay held until TransactionBehavior's commit finishes, not just until the handler returns.
        services.AddScoped<
            IPipelineBehavior<RefreshTokenCommand, AuthResult>,
            Identity.Application.Behaviors.RefreshTokenConcurrencyBehavior>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(moduleAssemblies);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            cfg.AddOpenBehavior(typeof(Identity.Application.Behaviors.TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(Catalog.Application.Behaviors.TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(Inventory.Application.Behaviors.TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(Customer.Application.Behaviors.TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(Cart.Application.Behaviors.TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(Pricing.Application.Behaviors.TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(Shipping.Application.Behaviors.TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(Payment.Application.Behaviors.TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(Order.Application.Behaviors.TransactionBehavior<,>));
            cfg.AddOpenBehavior(typeof(Review.Application.Behaviors.TransactionBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(moduleAssemblies);

        return services;
    }
}
