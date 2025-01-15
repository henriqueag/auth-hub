using AuthHub.Domain.Email;
using AuthHub.Domain.Users.Events;
using MassTransit;

namespace AuthHub.Application.EventHandlers.Users;

public class SendEmailOnPasswordRecoveryRequested(IEmailSender emailSender) : IConsumer<PasswordRecoveryEmailRequestedDomainEvent>
{
    public async Task Consume(ConsumeContext<PasswordRecoveryEmailRequestedDomainEvent> context)
    {
        var message = context.Message;
        var token = context.CancellationToken;

        await emailSender.SendEmailAsync(
            message.User.Email!, 
            EmailTemplateNames.PasswordRecovery,
            new Dictionary<string, string>
            {
                { "User", message.User.DisplayName },
                { "Link", message.Link },
                { "FullLink", message.Link }
            }, 
            token
        );
    }
}