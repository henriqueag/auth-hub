using AuthHub.Infrastructure.Data.Contexts;
using Serilog;

namespace AuthHub.Runtime.Host.Configurations;

public static class WebApplicationExtensions
{
    public static async Task ConfigurePipeline(this WebApplication app)
    {
        app.UseCors("AllowAll");

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        app.UseSerilogRequestLogging(opt => opt.IncludeQueryInRequestPath = true);

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        await app.Services.GetRequiredService<DatabaseInitializer>().InitializeAsync();

        app.MapEndpoints();
    }
}