namespace SampleSecurityProvider.ErrorHandling;

public class ProblemDetailsException(string code, int statusCode, string message) : Exception(message)
{
    public string Code { get; } = code;
    public int StatusCode { get; } = statusCode;
    public ICollection<Error> Errors { get; } = [];
}