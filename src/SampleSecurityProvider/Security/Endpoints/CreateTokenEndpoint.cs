using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SampleSecurityProvider.Models;
using SampleSecurityProvider.Security.Dtos;
using SampleSecurityProvider.Security.Services;

namespace SampleSecurityProvider.Security.Endpoints;

public class CreateTokenEndpoint : BaseSecurityEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        base.MapEndpoint(builder);

        builder.MapPost("token", CreateTokenAsync);
    }

    private static async Task<IResult> CreateTokenAsync(
        [FromForm] IDictionary<string, string?> payload, 
        UserManager<ApplicationUser> userManager,
        IAuthenticationService authService)
    {
        var tokenRequest = new TokenRequest(payload);
        
        if (tokenRequest.GrantType != "password")
        {
            return Results.BadRequest(new ProblemDetails
            {
                Type = "security.create-token.invalid-grant-type",
                Title = "Grant Type inválido",
                Status = StatusCodes.Status400BadRequest,
            });
        }
        
        var user = await userManager.FindByEmailAsync(tokenRequest.Email ?? string.Empty);
        if (user is null)
        {
            return Results.NotFound(new ProblemDetails
            {
                Type = "security.create-token.email-not-found",
                Title = "Email não encontrado",
                Status = StatusCodes.Status404NotFound,
            });
        }

        var token = await authService.CreateTokenAsync(user, tokenRequest.Password ?? string.Empty);
        return Results.Ok(token);
    }
}