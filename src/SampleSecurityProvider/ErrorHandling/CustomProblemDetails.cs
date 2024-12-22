using Microsoft.AspNetCore.Mvc;

namespace SampleSecurityProvider.ErrorHandling;

public class CustomProblemDetails : ProblemDetails
{
    private readonly List<Error> _errors = [];
    
    public CustomProblemDetails(string code, string message, int statusCode)
    {
        Type = code;
        Title = message;
        Status = statusCode;
        Extensions = new Dictionary<string, object?>
        {
            { "errors", _errors }
        };
    }
    
    public IReadOnlyList<Error> Errors => _errors;
    
    public void AddErrors(IEnumerable<Error> errors) => _errors.AddRange(errors);
}