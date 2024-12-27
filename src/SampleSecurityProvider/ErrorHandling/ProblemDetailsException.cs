namespace SampleSecurityProvider.ErrorHandling;

public class ProblemDetailsException(string code, string message, int statusCode) : Exception(message)
{
    public CustomProblemDetails ProblemDetails => new(code, Message, statusCode);
}