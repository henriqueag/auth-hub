using AuthHub.Application.Dtos.Security;
using AuthHub.Domain.Security.ValueObjects;
using MediatR;

namespace AuthHub.Application.Commands.Security.CreateToken;

public record CreateTokenCommand(TokenRequest Payload) : IRequest<SecurityToken>;