using AuthHub.Application.Commands.Users;
using AuthHub.Application.Commands.Users.ChangePassword;
using AuthHub.Domain.Email;
using AuthHub.Domain.Security.Repositories;
using AuthHub.Domain.Security.Services;
using AuthHub.Domain.Security.ValueObjects;
using AuthHub.Domain.Users.Entities;
using AuthHub.Infrastructure.Data.Contexts;
using AuthHub.Infrastructure.Data.Repositories;
using AuthHub.Infrastructure.Security.Services;
using AuthHub.Runtime.Host.ErrorHandling;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthHub.Runtime.Host.Configurations;

public static class ServiceCollectionExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddPresentation()
            .AddServices()
            .AddSecurity()
            .AddDatabase()
            .AddInMemoryBus()
            .AddErrorHandling();
    }
    
    private static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpoints();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
    
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()
            .AddScoped<IEmailSender, EmailSender>()
            ;
        
        services
            .AddSingleton<DatabaseInitializer>()
            .AddSingleton<IJwksManager, JwksManager>()
            .AddSingleton<ISecurityTokenManager, SecurityTokenManager>()
            .AddSingleton<IEmailTemplateReader, EmailTemplateReader>()
            ;
     
        services.AddValidatorsFromAssembly(typeof(ChangePasswordCommand).Assembly);
        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();
        
        services.ConfigureOptions<SmtpOptionsSetup>();
        
        return services;
    }
    
    private static IServiceCollection AddSecurity(this IServiceCollection services)
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
            .SetApplicationName("AuthHub");

        services.AddCors(options =>
            options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
        );

        services
            .ConfigureOptions<JwtOptionsSetup>()
            .ConfigureOptions<JwtBearerOptionsSetup>();
        
        return services;
    }
 
    private static IServiceCollection AddDatabase(this IServiceCollection services)
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

    private static IServiceCollection AddInMemoryBus(this IServiceCollection services)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.AddConsumers(typeof(ServiceCollectionExtensions).Assembly);

            busConfigurator.UsingInMemory((context, configurator) =>
            {
                configurator.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }
    
    private static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        services
            .AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>();
        
        return services;
    }
}