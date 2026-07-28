using ECommerce.API.Seed;
using Scalar.AspNetCore;
using Serilog;

namespace ECommerce.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task UseApiPipelineAsync(this WebApplication app)
    {
        await app.MigrateModuleDatabasesAsync();

        app.UseForwardedHeaders();

        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                diagnosticContext.Set("RemoteIp", httpContext.Connection.RemoteIpAddress?.ToString());
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms from {RemoteIp}";
        });

        app.UseRateLimiter();

        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        // Scalar/OpenAPI stay mapped in every environment — this server doubles as the shared
        // API reference for the web/mobile teammates building against it, not just a public
        // production endpoint. Only the sample Admin/Customer seed data stays Development-only.
        app.MapOpenApi();
        app.MapScalarApiReference();

        if (app.Environment.IsDevelopment())
        {
            await DevelopmentDataSeeder.SeedAsync(app.Services);
        }
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }

        app.UseCors("TeamDev");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }
}
