using AuthHub.Domain.Abstractions;
using AuthHub.Domain.Security.Services;
using AuthHub.Domain.Security.ValueObjects;
using AuthHub.Domain.Users.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Commands.Security.CreateToken;

public class CreateTokenCommandHandler(UserManager<User> userManager, IAuthenticationService authService) : IRequestHandler<CreateTokenCommand, SecurityToken>
{
    public async Task<SecurityToken> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        if (request.Payload.GrantType != "password")
        {
            throw new ProblemDetailsException(
                "application.commands.security.create-token.invalid-grant-type",
                "O grant_type fornecido é inválido.", Error.BadRequest);
        }
        
        var user = await userManager.FindByEmailAsync(request.Payload.Email ?? string.Empty);
        if (user is null)
        {
            throw new ProblemDetailsException(
                "application.commands.security.create-token.email-not-found",
                "O email fornecido não foi encontrado", Error.NotFound);
        }

        var token = await authService.CreateTokenAsync(user, request.Payload.Password ?? string.Empty);
        return token;
    }
}