using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SampleSecurityProvider.ErrorHandling;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ProblemDetailsException ex) return false;

        var problemDetails = new ProblemDetails
        {
            Type = ex.Code,
            Title = ex.Message,
            Status = ex.StatusCode,
            Extensions = new Dictionary<string, object?>
            {
                { "errors", ex.Errors }
            }
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = ex,
            ProblemDetails = problemDetails,
        });
    }
}