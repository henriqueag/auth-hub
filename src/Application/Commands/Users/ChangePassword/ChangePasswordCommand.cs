using AuthHub.Application.Dtos.Users;
using MediatR;

namespace AuthHub.Application.Commands.Users.ChangePassword;

public record ChangePasswordCommand : PasswordRequest, IRequest;