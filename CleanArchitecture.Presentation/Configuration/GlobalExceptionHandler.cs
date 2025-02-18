using Microsoft.AspNetCore.Diagnostics;
using CleanArchitecture.Application.Common.Exceptions;

namespace CleanArchitecture.Presentation.Configuration;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
        _exceptionHandlers = new()
        {
            { typeof(CommonValidationException), HandleValidationException }
        };
    }


    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        Type exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out Func<HttpContext, Exception, Task>? exceptionHandler))
        {
            await exceptionHandler.Invoke(httpContext, exception);
            return true;
        }

        _logger.LogError(exception, "An error occurred while processing the request.");
        
        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request.",
            Detail = exception.Message
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }

    private async Task HandleValidationException(HttpContext httpContext, Exception ex)
    {
        CommonValidationException exception = (CommonValidationException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails(exception.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        });
    }
}