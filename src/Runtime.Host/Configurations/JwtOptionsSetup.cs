using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AuthHub.Domain.Security.Options;

public class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtOptions>
{
    public void Configure(JwtOptions options)
    {
        configuration.GetSection("Jwt").Bind(options);
    }
}