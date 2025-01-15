using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SampleSecurityProvider.Security.Services;

namespace AuthHub.Domain.Security.Options;

public class JwtBearerOptionsSetup(IOptions<JwtOptions> jwtOptions, IJwksManager jwksManager) : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(JwtBearerOptions options)
    {
        Configure("", options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.IncludeErrorDetails = true;
        options.MapInboundClaims = false;
        options.Audience = jwtOptions.Value.Audience;
        
        options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
        options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Sub;
        options.TokenValidationParameters.ValidAudience = jwtOptions.Value.Audience;
        options.TokenValidationParameters.ValidIssuer = jwtOptions.Value.Issuer;
        options.TokenValidationParameters.IssuerSigningKey = jwksManager.GetPublicKey();
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.LifetimeValidator = (_, expires, __, ___) => expires is not null && expires > DateTime.UtcNow;
    }
}