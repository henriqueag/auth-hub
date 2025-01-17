using AuthHub.Infrastructure.Email;
using Microsoft.Extensions.Options;

namespace AuthHub.Runtime.Host.Configurations;

public class SmtpOptionsSetup(IConfiguration configuration) : IConfigureOptions<SmtpOptions>
{
    public void Configure(SmtpOptions options)
    {
        configuration.GetSection("Smtp").Bind(options);
    }
}