using Microsoft.AspNetCore.Authentication.JwtBearer;
using SampleSecurityProviderUI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) => 
    logger.ReadFrom.Configuration(context.Configuration)
);

builder.Services
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
);

builder.Services
    .ConfigureOptions<JwtOptionsSetup>()
    .ConfigureOptions<JwtBearerOptionsSetup>();

var app = builder.Build();

app.UseSerilogRequestLogging(opt => opt.IncludeQueryInRequestPath = true);

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");

app.MapGet("api/test", () => "Hello World!").RequireAuthorization();

app.Run();