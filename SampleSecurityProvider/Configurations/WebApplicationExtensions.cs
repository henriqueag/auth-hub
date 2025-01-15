using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.EntityFramework.DbContext;
using Serilog;

namespace SampleSecurityProvider.Configurations;

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