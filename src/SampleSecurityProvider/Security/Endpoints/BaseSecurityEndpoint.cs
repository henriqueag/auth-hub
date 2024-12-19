using SampleSecurityProvider.Abstractions;

namespace SampleSecurityProvider.Security.Endpoints;

public class BaseSecurityEndpoint : IEndpoint
{
    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGroup("api/security");
    }
}