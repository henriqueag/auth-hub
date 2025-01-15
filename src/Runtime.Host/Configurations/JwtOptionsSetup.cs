using AuthHub.Domain.Security.Options;
using Microsoft.Extensions.Options;

namespace AuthHub.Runtime.Host.Configurations;

public class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtOptions>
{
    public void Configure(JwtOptions options)
    {
        configuration.GetSection("Jwt").Bind(options);
    }
}