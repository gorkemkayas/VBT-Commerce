using System.Reflection;
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
using Order.Application.Commands.Checkout.PlaceMyOrder;
using Order.Infrastructure;
using Payment.Application.Commands.Charges.ChargeOrderPayment;
using Payment.Infrastructure;
using Pricing.Application.Commands.Prices.CreatePrice;
using Pricing.Infrastructure;
using Review.Application.Commands.Me.CreateMyReview;
using Review.Infrastructure;
using Scalar.AspNetCore;
using Shipping.Application.Commands.ShippingCompanies.CreateShippingCompany;
using Shipping.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.Local.json",
    optional: true,
    reloadOnChange: true);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Modules
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);
builder.Services.AddCustomerModule(builder.Configuration);
builder.Services.AddCartModule(builder.Configuration);
builder.Services.AddPricingModule(builder.Configuration);
builder.Services.AddShippingModule(builder.Configuration);
builder.Services.AddPaymentModule(builder.Configuration);
builder.Services.AddOrderModule(builder.Configuration);
builder.Services.AddReviewModule(builder.Configuration);

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
    typeof(CreateMyReviewCommand).Assembly
};

// Registered before the open behaviors below so it becomes the outermost wrapper — its lock
// must stay held until TransactionBehavior's commit finishes, not just until the handler returns.
builder.Services.AddScoped<
    IPipelineBehavior<RefreshTokenCommand, AuthResult>,
    Identity.Application.Behaviors.RefreshTokenConcurrencyBehavior>();

builder.Services.AddMediatR(cfg =>
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

builder.Services.AddValidatorsFromAssemblies(moduleAssemblies);

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
if(!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
