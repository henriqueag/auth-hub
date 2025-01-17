namespace AuthHub.Domain.Abstractions;

public class ProblemDetailsException(string code, string message, int statusCode, IEnumerable<Error>? errors = null) : Exception(message)
{
    public string Code { get; } = code;
    public int StatusCode { get; } = statusCode;
    public IEnumerable<Error> Errors { get; } = errors ?? [];
}