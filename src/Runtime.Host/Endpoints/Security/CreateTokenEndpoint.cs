using AuthHub.Application.Commands.Security.CreateToken;
using AuthHub.Application.Dtos.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Security;

public class CreateTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/security/token", CreateTokenAsync)
            .WithTags("Security")
            .WithOpenApi()
            .DisableAntiforgery();
    }
    
    private static async Task<IResult> CreateTokenAsync(
        [FromServices] ISender sender,
        IFormCollection payload,
        CancellationToken cancellationToken)
    {
        var command = new CreateTokenCommand(new TokenRequest(payload));
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
}