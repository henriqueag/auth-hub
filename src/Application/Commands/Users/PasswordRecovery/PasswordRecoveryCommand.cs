using AuthHub.Application.Dtos.Users;
using MediatR;

namespace AuthHub.Application.Commands.Users.PasswordRecovery;

public record PasswordRecoveryCommand(string Email, string Token) : PasswordRequest, IRequest;