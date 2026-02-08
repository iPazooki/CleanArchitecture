using Microsoft.AspNetCore.Diagnostics;

namespace CleanArchitecture.Api.Configuration;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly Dictionary<Type, Func<HttpContext, Exception, CancellationToken, Task>> _exceptionHandlers;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
        _exceptionHandlers = new Dictionary<Type, Func<HttpContext, Exception, CancellationToken, Task>>
        {
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException }
        };
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(httpContext);

        Type exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out Func<HttpContext, Exception, CancellationToken, Task>? exceptionHandler))
        {
            await exceptionHandler.Invoke(httpContext, exception, cancellationToken).ConfigureAwait(false);
            return true;
        }

        _logger.LogError(exception, "An error occurred while processing the request {DateTime} {Path}", DateTimeOffset.UtcNow, httpContext.Request.Path);

        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "An error occurred while processing your request.",
            Detail = exception.Message
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return true;
    }

    private async Task HandleUnauthorizedAccessException(HttpContext httpContext, Exception ex, CancellationToken cancellationToken)
    {
        _logger.LogWarning(ex, "An error occurred while processing the request {DateTime} {Path}", DateTimeOffset.UtcNow, httpContext.Request.Path);

        await httpContext.Response
            .WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized Access",
                    Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1"
                }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
