using System.Text.Json.Serialization;
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
    
    public CustomProblemDetails(string code, string message, int statusCode, IEnumerable<Error> errors) : this(code, message, statusCode)
    {
        _errors.AddRange(errors);
    }
}