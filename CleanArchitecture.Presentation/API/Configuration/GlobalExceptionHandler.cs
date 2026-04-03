using Microsoft.AspNetCore.Diagnostics;

namespace CleanArchitecture.Api.Configuration;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(httpContext);

        if (exception is UnauthorizedAccessException)
        {
            await HandleUnauthorizedAccessException(httpContext, cancellationToken).ConfigureAwait(false);
            return true;
        }

        logger.LogError(exception, "An error occurred while processing the request {DateTime} {Path}", DateTimeOffset.UtcNow, httpContext.Request.Path);

        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "An error occurred while processing your request.",
            Detail = environment.IsDevelopment() ? exception.Message : null
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return true;
    }

    private static async Task HandleUnauthorizedAccessException(HttpContext httpContext, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

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