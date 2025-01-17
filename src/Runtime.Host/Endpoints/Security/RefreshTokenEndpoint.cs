using AuthHub.Application.Commands.Security.RefreshToken;
using AuthHub.Application.Dtos.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Security;

public class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/security/refresh-token", RefreshTokenAsync)
            .WithTags("Security")
            .WithOpenApi()
            .DisableAntiforgery();
    }
    
    private static async Task<IResult> RefreshTokenAsync(
        [FromServices] ISender sender,
        IFormCollection payload,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(new TokenRequest(payload));
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
}