using AuthHub.Application.Dtos.Security;
using AuthHub.Domain.Security.ValueObjects;
using MediatR;

namespace AuthHub.Application.Commands.Security.RefreshToken;

public record RefreshTokenCommand(TokenRequest Payload) : IRequest<SecurityToken>;