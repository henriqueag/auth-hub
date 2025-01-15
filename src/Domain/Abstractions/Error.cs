using System.Net;

namespace AuthHub.Domain.Abstractions;

public record Error(string Code, string Message, IEnumerable<Error>? Errors = null)
{
    public const int NotFound = (int)HttpStatusCode.NotFound;
    public const int BadRequest = (int)HttpStatusCode.BadRequest;
    public const int Unauthorized = (int)HttpStatusCode.Unauthorized;
}