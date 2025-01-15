using AuthHub.Application.Dtos.Users;
using AuthHub.Domain.Abstractions;
using AuthHub.Domain.Users.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Commands.Users.PasswordRecovery;

public class PasswordRecoveryCommandHandler(
    UserManager<User> userManager, 
    IValidator<PasswordRequest> validator)
    : IRequestHandler<PasswordRecoveryCommand>
{
    public async Task Handle(PasswordRecoveryCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            throw new ProblemDetailsException(
                "application.commands.users.password-recovery.not-found",
                $"Nenhum usuário não encontrado com o email {request.Email}.",
                Error.NotFound);
        } 
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ProblemDetailsException(
                "application.commands.users.password-recovery.invalid-data",
                "Os dados fornecidos para a recuperação de senha do usuário estão inválidos.",
                Error.BadRequest, validationResult.Errors.Select(error => new Error(error.ErrorCode, error.ErrorMessage)));
        }
        
        var resetResult = await userManager.ResetPasswordAsync(user, request.Token, request.Password);
        if (!resetResult.Succeeded)
        {
            throw new ProblemDetailsException(
                "application.commands.users.password-recovery-failed",
                resetResult.Errors.Select(err => err.Description).First(),
                Error.BadRequest);
        }
    }
}