using AuthHub.Domain.Security.Services;

namespace AuthHub.Runtime.Host.Endpoints.Security;

public class JwksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet(".well-known/jwks.json", (IJwksManager jwksManager) =>
            {
                var jwks = jwksManager.GetJwks();
                var result = new
                {
                    Keys = jwks.Keys
                        .Select(key => new { key.Kty, key.Kid, key.Use, key.Alg, key.N, key.E })
                        .ToList()
                };
                return Results.Ok(result);
            })
            .WithTags("Security")
            .WithOpenApi();
    }
}