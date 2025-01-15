using AuthHub.Domain.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.ErrorHandling;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ProblemDetailsException ex) return false;
        
        httpContext.Response.StatusCode = ex.StatusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = ex,
            ProblemDetails = new ProblemDetails
            {
                Type = ex.Code,
                Title = ex.Message,
                Status = ex.StatusCode,
                Extensions = new Dictionary<string, object?>
                {
                    { "errors", ex.Errors }
                }
            }
        });
    }
}