using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SampleSecurityProvider.Models;
using SampleSecurityProvider.Security.Dtos;
using SampleSecurityProvider.Security.Services;

namespace SampleSecurityProvider.Security.Endpoints;

public class RefreshTokenEndpoint : BaseSecurityEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        base.MapEndpoint(app);

        app.MapPost("refresh-token", RefreshTokenAsync);
    }

    private static async Task<IResult> RefreshTokenAsync(
        [FromForm] IDictionary<string, string?> payload,
        UserManager<ApplicationUser> userManager,
        IAuthenticationService authService)
    {
        var tokenRequest = new TokenRequest(payload);
        
        if (tokenRequest.GrantType != "refresh_token")
        {
            return Results.BadRequest("Invaild grant type");
        }

        if (string.IsNullOrEmpty(tokenRequest.RefreshToken))
        {
            return Results.BadRequest("Refresh token required");
        }
        
        var token = await authService.RefreshTokenAsync(user, tokenRequest.RefreshToken);
        return Results.Ok(token);
    }
}