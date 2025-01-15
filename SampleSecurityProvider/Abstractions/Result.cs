using SampleSecurityProvider.ErrorHandling;

namespace SampleSecurityProvider.Abstractions;

public class Result
{
    public bool IsSuccess { get; private init; }
    public CustomProblemDetails? Error { get; private init; }
    
    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(CustomProblemDetails error) => new() { IsSuccess = false, Error = error };
}