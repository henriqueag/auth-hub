using System.Web;
using AuthHub.Domain.Abstractions;
using AuthHub.Domain.Users.Entities;
using AuthHub.Domain.Users.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthHub.Application.Commands.Users.SendPasswordRecoveryEmail;

public class SendPasswordRecoveryEmailCommandHandler(UserManager<User> userManager, IBus bus) : IRequestHandler<SendPasswordRecoveryEmailCommand>
{
    public async Task Handle(SendPasswordRecoveryEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email ?? "");
        if (user is null)
        {
            throw new ProblemDetailsException(
                "application.commands.users.password-recovery.user-not-found",
                "Não foi possível localizar um usuário com o e-mail fornecido.",
                Error.NotFound);
        }

        if (!user.Active)
        {
            throw new ProblemDetailsException(
                "application.commands.users.password-recovery.user-inactive",
                "O usuário associado ao e-mail fornecido está inativo e não pode realizar a recuperação de senha.",
                Error.BadRequest);
        }
        
        var recoveryToken = await userManager.GeneratePasswordResetTokenAsync(user);
        
        var link = $"{request.Link}?email={user.Email}&token={HttpUtility.UrlEncode(recoveryToken)}";

        await bus.Publish(new PasswordRecoveryEmailRequestedDomainEvent(user, link) as object, cancellationToken);
    }
}