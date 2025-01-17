using AuthHub.Runtime.Host.Configurations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) => 
    logger.ReadFrom.Configuration(context.Configuration)
);

builder.Services.ConfigureServices();

var app = builder.Build();

await app.ConfigurePipeline();

await app.RunAsync();