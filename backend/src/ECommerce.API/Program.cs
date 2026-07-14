using System.Reflection;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.Security;
using ECommerce.API.Middleware;
using ECommerce.API.Security;
using FluentValidation;
using Identity.Application.Commands.Register;
using Identity.Infrastructure;
using Scalar.AspNetCore;

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

// CQRS: MediatR + FluentValidation + Pipeline Behaviors (module assemblies register their own handlers/validators)
var moduleAssemblies = new[] { typeof(RegisterCommand).Assembly };

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(moduleAssemblies);
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
    cfg.AddOpenBehavior(typeof(Identity.Application.Behaviors.TransactionBehavior<,>));
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
