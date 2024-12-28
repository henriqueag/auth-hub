using MassTransit;
using SampleSecurityProvider.Email;
using SampleSecurityProvider.Users.Events;

namespace SampleSecurityProvider.Users.Consumers;

public class SendEmailOnPasswordRecoveryRequested(IEmailSender emailSender) : IConsumer<PasswordRecoveryEmailRequested>
{
    public async Task Consume(ConsumeContext<PasswordRecoveryEmailRequested> context)
    {
        var message = context.Message;
        var token = context.CancellationToken;

        await emailSender.SendEmailAsync(
            message.User.Email!, 
            EmailTemplateNames.PasswordRecovery,
            new Dictionary<string, string>
            {
                { "User", message.User.DisplayName },
                { "Link", message.Link }
            }, 
            token
        );
    }
}