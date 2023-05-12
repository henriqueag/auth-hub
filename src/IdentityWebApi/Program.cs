using System.Text;
using IdentityWebApi.EntityFramework;
using IdentityWebApi.Models;
using IdentityWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostBuilderContext, logger) => logger
    .ReadFrom.Configuration(hostBuilderContext.Configuration)
    .WriteTo.Async(config => config.Console())
);

builder.Services.AddLogging();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireDigit = true;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.IncludeErrorDetails = true;
            options.Audience = "http://localhost:5142";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,

                ValidAudience = "http://localhost:5142",
                ValidateAudience = true,

                ValidIssuer = "http://localhost:5142",
                ValidateIssuer = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("b2c1c619fc6b404598349fded6b93ee7")),
                ValidateIssuerSigningKey = true,

                LifetimeValidator = CustomLifetimeValidator,
                ValidateLifetime = true
            };

            static bool CustomLifetimeValidator(
                DateTime? notBefore,
                DateTime? expires,
                SecurityToken securityToken,
                TokenValidationParameters validationParameters)
                    => expires is not null && expires > DateTime.UtcNow;
        });

builder.Services.AddDataProtection()
    .SetApplicationName("IdentityWebApi");

builder.Services.AddCors();
builder.Services.AddRouting(opt => opt.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ITokenService, TokenService>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(policy =>
{
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
    policy.AllowAnyOrigin();
});

app.MapControllers();

app.Run();
