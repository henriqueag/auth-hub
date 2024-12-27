using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.EntityFramework.DbContext;
using SampleSecurityProvider.EntityFramework.Repositories;
using SampleSecurityProvider.ErrorHandling;
using SampleSecurityProvider.Security.Options;
using SampleSecurityProvider.Security.Repositories;
using SampleSecurityProvider.Security.Services;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddDefaultTokenProviders()
            .AddErrorDescriber<PortugueseIdentityErrorDescriber>()
            .AddEntityFrameworkStores<SqliteDbContext>();
        
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services.AddAuthorization();
        
        services
            .AddDataProtection()
            .SetApplicationName("SampleSecurityProvider");

        services.AddCors(options =>
            options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
        );

        services
            .ConfigureOptions<JwtOptionsSetup>()
            .ConfigureOptions<JwtBearerOptionsSetup>();
        
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        
        services
            .AddSingleton<DatabaseInitializer>()
            .AddSingleton<IJwksManager, JwksManager>()
            .AddSingleton<ISecurityTokenManager, SecurityTokenManager>();
     
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        
        return services;
    }
    
    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        services
            .AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>();
        
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<SqliteDbContext>(
            optionsAction: (_, ctxBuilder) =>
            {
                var dbPath = Path.Combine(AppContext.BaseDirectory, "db");
                if (!Directory.Exists(dbPath))
                {
                    Directory.CreateDirectory(dbPath);
                }
        
                ctxBuilder.UseSqlite($"Data Source={dbPath}/db.sqlite");
            });

        return services;
    }

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpoints();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}