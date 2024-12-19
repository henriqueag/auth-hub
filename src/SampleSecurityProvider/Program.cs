using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SampleSecurityProvider.EntityFramework.DbContext;
using SampleSecurityProvider.Models;
using SampleSecurityProvider.Security.Options;
using SampleSecurityProvider.Security.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) => 
    logger.ReadFrom.Configuration(context.Configuration)
);

builder.Services.AddDbContext<SqliteDbContext>(
    optionsAction: (_, ctxBuilder) =>
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "db");
        if (!Directory.Exists(dbPath))
        {
            Directory.CreateDirectory(dbPath);
        }
        
        ctxBuilder.UseSqlite($"Data Source={dbPath}/db.sqlite");
    });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireUppercase = false;
        options.Password.RequiredUniqueChars = 0;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<SqliteDbContext>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer();

builder.Services
    .AddDataProtection()
    .SetApplicationName("SampleSecurityProvider");

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<DatabaseInitializer>()
    .AddSingleton<IJwksManager, JwksManager>()
    .AddSingleton<ISecurityTokenManager, SecurityTokenManager>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services
    .ConfigureOptions<JwtOptionsSetup>()
    .ConfigureOptions<JwtBearerOptionsSetup>();

var app = builder.Build();

app.UseSerilogRequestLogging(opt => opt.IncludeQueryInRequestPath = true);

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");

await app.Services.GetRequiredService<DatabaseInitializer>().InitializeAsync();

app.MapControllers();

await app.RunAsync();
