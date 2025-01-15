using Microsoft.Extensions.Options;

namespace SampleSecurityProvider.Email;

public class SmtpOptionsSetup(IConfiguration configuration) : IConfigureOptions<SmtpOptions>
{
    public void Configure(SmtpOptions options)
    {
        configuration.GetSection("Smtp").Bind(options);
    }
}