using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthHub.Domain.Email;

public class EmailSender(
    IOptions<SmtpOptions> smtpOptions,
    IEmailTemplateReader templateReader,
    ILogger<EmailSender> logger
    ) : IEmailSender
{
    public async Task SendEmailAsync(string to, string templateName, IDictionary<string, string> templateParameters, CancellationToken cancellationToken)
    {
        using var smtp = new SmtpClient(smtpOptions.Value.Server, smtpOptions.Value.Port);
        
        smtp.Credentials = new NetworkCredential(smtpOptions.Value.Username, smtpOptions.Value.Password);
        smtp.EnableSsl = true;

        var templateInfo = templateReader.GetTemplate(templateName, templateParameters);
        
        var message = new MailMessage(smtpOptions.Value.Sender, to)
        {
            Body = templateInfo.Html,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = true,
            Subject = templateInfo.Subject,
            SubjectEncoding = Encoding.UTF8
        };

        try
        {
            await smtp.SendMailAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao enviar o e-mail");
        }
    }
}