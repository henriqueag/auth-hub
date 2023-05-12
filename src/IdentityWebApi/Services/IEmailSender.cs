using SendGrid;
using SendGrid.Helpers.Mail;

namespace IdentityWebApi.Services;

public interface IEmailSender
{
    Task SendAsync(string subject, string message, string toEmail, CancellationToken cancellationToken);
}

public class EmailSender : IEmailSender
{
    private const string _sendGridApiKey = "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";

    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    public async Task SendAsync(string subject, string message, string toEmail, CancellationToken cancellationToken)
    {
        var client = new SendGridClient(_sendGridApiKey);
        var msg = new SendGridMessage
        {
            From = new EmailAddress("henriquesantos.ag2@gmail.com", "Recuperação de senha"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };

        msg.AddTo(new EmailAddress(toEmail));
        msg.SetClickTracking(false, false);

        var response = await client.SendEmailAsync(msg, cancellationToken);

        _logger.LogInformation(
            response.IsSuccessStatusCode
            ? $"Email to {toEmail} queued successfully!"
            : $"Failure Email to {toEmail}");
    }
}
