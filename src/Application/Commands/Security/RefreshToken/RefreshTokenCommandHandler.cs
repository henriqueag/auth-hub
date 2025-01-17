using AuthHub.Domain.Abstractions;
using AuthHub.Domain.Security.Services;
using AuthHub.Domain.Security.ValueObjects;
using MediatR;

namespace AuthHub.Application.Commands.Security.RefreshToken;

public class RefreshTokenCommandHandler(IAuthenticationService authService) : IRequestHandler<RefreshTokenCommand, SecurityToken>
{
    public async Task<SecurityToken> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (request.Payload.GrantType != "refresh_token")
        {
            throw new ProblemDetailsException(
                "application.commands.security.refresh-token.invalid-grant-type",
                "O grant_type fornecido é inválido.", Error.BadRequest);
        }

        if (string.IsNullOrEmpty(request.Payload.RefreshToken))
        {
            throw new ProblemDetailsException(
                "application.commands.security.refresh-token.refresh-token-required",
                "O refresh token é obrigatório.", Error.BadRequest);
        }
        
        var token = await authService.RefreshTokenAsync(request.Payload.RefreshToken);
        return token;
    }
}