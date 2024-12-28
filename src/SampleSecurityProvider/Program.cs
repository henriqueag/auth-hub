using SampleSecurityProvider.Configurations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) => 
    logger.ReadFrom.Configuration(context.Configuration)
);

builder.Services
    .AddPresentation()
    .AddDatabase()
    .AddErrorHandling()
    .AddSecurity()
    .AddServices()
    .AddInMemoryBus();

var app = builder.Build();

await app.ConfigurePipeline();

await app.RunAsync();