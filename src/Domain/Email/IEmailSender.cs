namespace AuthHub.Domain.Email;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string templateName, IDictionary<string, string> templateParameters, CancellationToken cancellationToken);
}