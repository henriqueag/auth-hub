using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.Security.Dtos;
using SampleSecurityProvider.Security.Services;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Security.Endpoints;

public class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/security/refresh-token", RefreshTokenAsync)
            .WithTags("Security")
            .WithOpenApi()
            .DisableAntiforgery()
            ;
    }

    private static async Task<IResult> RefreshTokenAsync(
        IFormCollection payload,
        UserManager<User> userManager,
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
        
        var token = await authService.RefreshTokenAsync(tokenRequest.RefreshToken);
        return Results.Ok(token);
    }
}