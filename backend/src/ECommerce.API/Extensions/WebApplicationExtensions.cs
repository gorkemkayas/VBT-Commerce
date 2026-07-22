using ECommerce.API.Seed;
using Scalar.AspNetCore;
using Serilog;

namespace ECommerce.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task UseApiPipelineAsync(this WebApplication app)
    {
        app.UseForwardedHeaders();

        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
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
