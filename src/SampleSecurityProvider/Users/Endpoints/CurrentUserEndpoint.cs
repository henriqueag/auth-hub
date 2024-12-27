using SampleSecurityProvider.Abstractions;

namespace SampleSecurityProvider.Users.Endpoints;

public class CurrentUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/current-user", (HttpContext context) =>
        {
            var user = context.User.Identity;
            return "Hii!";
        });
    }
}