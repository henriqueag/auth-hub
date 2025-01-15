using AuthHub.Domain.Abstractions;
using AuthHub.Domain.Users.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Commands.Users;

public record ChangePasswordCommand : IRequest
{
    public string? CurrentPassword { get; init; }
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}

public class ChangePasswordCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager,
    IValidator<ChangePasswordCommand> validator)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var principal = httpContextAccessor.HttpContext!.User;
        var user = (await userManager.FindByIdAsync(principal.Identity!.Name!))!;

        if (!await userManager.CheckPasswordAsync(user, request.CurrentPassword!))
        {
            throw new ProblemDetailsException(
                "application.commands.users.change-password.invalid-current-password",
                "A senha atual fornecida não corresponde à senha registrada para o usuário.",
                Error.BadRequest);
        }
        
        if (request.Password != request.ConfirmPassword)
        {
            throw new ProblemDetailsException(
                "application.commands.users.change-password.invalid-current-password",
                "A senha atual fornecida não corresponde à senha registrada para o usuário.",
                Error.BadRequest
                )
            
        }
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ProblemDetailsException(
                "application.commands.users.change-password.invalid-data",
                "Os dados fornecidos para a troca de senha do usuário estão inválidos.",
                Error.BadRequest, validationResult.Errors.Select(error => new Error(error.ErrorCode, error.ErrorMessage)));
        }
        
        var changeResult = await userManager.ChangePasswordAsync(user, request.CurrentPassword!, request.Password);
        if (!changeResult.Succeeded)
        {
            throw new ProblemDetailsException(
                "application.commands.users.change-password-failed",
                "Não foi possível trocar a senha, verifique os detalhes.",
                Error.BadRequest, changeResult.Errors.Select(err => new Error(err.Code, err.Description)));
        }
    }
}
