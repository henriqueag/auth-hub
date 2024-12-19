using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SampleSecurityProviderUI;

public class JwtBearerOptionsSetup(
    IHttpClientFactory factory,
    IOptions<JwtOptions> jwtOptions
    ) : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(JwtBearerOptions options)
    {
        throw new NotImplementedException();
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
        options.TokenValidationParameters.IssuerSigningKeys = GetSigningKey();
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.LifetimeValidator = (_, expires, __, ___) => expires is not null && expires > DateTime.UtcNow;
    }

    private IList<SecurityKey> GetSigningKey()
    {
        var client = factory.CreateClient(jwtOptions.Value.Issuer);
        
        var request = new HttpRequestMessage(HttpMethod.Get, jwtOptions.Value.JwksUri);
        var response = client.SendAsync(request).GetAwaiter().GetResult();

        var jwksJson = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var jwks = JsonWebKeySet.Create(jwksJson);

        return jwks.GetSigningKeys();
    }
}