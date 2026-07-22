using ECommerce.API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.Local.json",
    optional: true,
    reloadOnChange: true);

// Console-only default logging is lost on every VPS process restart, so Serilog persists
// structured logs to rolling daily files as well — the "Serilog" appsettings section drives
// sinks/levels per environment without a redeploy.
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

builder.Services
    .AddApiServices(builder.Configuration)
    .AddModules(builder.Configuration)
    .AddCqrs();

var app = builder.Build();

await app.UseApiPipelineAsync();

app.Run();
