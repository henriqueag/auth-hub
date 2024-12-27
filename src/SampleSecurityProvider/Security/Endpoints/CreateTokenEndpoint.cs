using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SampleSecurityProvider.Abstractions;
using SampleSecurityProvider.ErrorHandling;
using SampleSecurityProvider.Security.Dtos;
using SampleSecurityProvider.Security.Services;
using SampleSecurityProvider.Users.Entities;

namespace SampleSecurityProvider.Security.Endpoints;

public class CreateTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/security/token", CreateTokenAsync)
            .WithTags("Security")
            .WithOpenApi()
            .DisableAntiforgery()
            ;
    }

    private static async Task<IResult> CreateTokenAsync(
        IFormCollection payload,
        UserManager<User> userManager,
        IAuthenticationService authService)
    {
        
        var tokenRequest = new TokenRequest(payload);
        
        if (tokenRequest.GrantType != "password")
        {
            return Results.BadRequest(new CustomProblemDetails(
                "security.create-token-endpoint.invalid-grant-type",
                "O grant_type fornecido é inválido.",
                StatusCodes.Status400BadRequest)
            );
        }
        
        var user = await userManager.FindByEmailAsync(tokenRequest.Email ?? string.Empty);
        if (user is null)
        {
            return Results.NotFound(new CustomProblemDetails(
                "security.create-token-endpoint.email-not-found",
                "O email fornecido não foi encontrado",
                StatusCodes.Status404NotFound)
            );
        }

        var token = await authService.CreateTokenAsync(user, tokenRequest.Password ?? string.Empty);
        return Results.Ok(token);
    }
}