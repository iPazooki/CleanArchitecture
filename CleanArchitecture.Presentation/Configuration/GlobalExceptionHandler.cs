using Microsoft.AspNetCore.Diagnostics;
using CleanArchitecture.Application.Common.Exceptions;

namespace CleanArchitecture.Presentation.Configuration;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly Dictionary<Type, Func<HttpContext, Exception, CancellationToken, Task>> _exceptionHandlers;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
        _exceptionHandlers = new Dictionary<Type, Func<HttpContext, Exception, CancellationToken, Task>>
        {
            { typeof(ApplicationValidationException), HandleApplicationException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException }
        };
    }


    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred while processing the request.");

        Type exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out Func<HttpContext, Exception, CancellationToken, Task>? exceptionHandler))
        {
            await exceptionHandler.Invoke(httpContext, exception, cancellationToken);
            return true;
        }

        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "An error occurred while processing your request.",
            Detail = exception.Message
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }

    private async Task HandleApplicationException(HttpContext httpContext, Exception ex, CancellationToken cancellationToken)
    {
        ApplicationValidationException exception = (ApplicationValidationException)ex;

        //httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        
        ValidationProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            Title = "An error occurred while processing your request.",
            Detail = exception.Message,
            Errors = exception.Errors
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
    }
    
    private async Task HandleUnauthorizedAccessException(HttpContext httpContext, Exception ex, CancellationToken cancellationToken) =>
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
        }, cancellationToken: cancellationToken);
}