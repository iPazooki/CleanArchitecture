using Microsoft.AspNetCore.Diagnostics;

namespace CleanArchitecture.Api.Configuration;

/// <summary>
/// Turns an unhandled exception into a ProblemDetails response.
/// </summary>
/// <remarks>
/// Responses are written through <see cref="IProblemDetailsService"/> so they pick up the
/// registered customisations — notably the <c>traceId</c> that ties a response back to its trace.
/// </remarks>
internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    private const string ForbiddenType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4";
    private const string ServerErrorType = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(httpContext);

        // 403, not 401: the caller was authenticated, they simply are not allowed to do this.
        // A 401 would ask them to present credentials they have already presented.
        if (exception is UnauthorizedAccessException)
        {
            return await WriteAsync(
                httpContext,
                exception: null,
                StatusCodes.Status403Forbidden,
                "Forbidden",
                ForbiddenType).ConfigureAwait(false);
        }

        logger.UnhandledException(httpContext.Request.Path, exception);

        return await WriteAsync(
            httpContext,
            exception,
            StatusCodes.Status500InternalServerError,
            "An error occurred while processing your request.",
            ServerErrorType).ConfigureAwait(false);
    }

    private async ValueTask<bool> WriteAsync(
        HttpContext httpContext,
        Exception? exception,
        int statusCode,
        string title,
        string type)
    {
        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,

                // Exception text belongs in the logs. It only reaches a client in Development.
                Detail = environment.IsDevelopment() ? exception?.Message : null
            }
        }).ConfigureAwait(false);
    }
}

/// <summary>
/// Source-generated log messages for the exception handler.
/// </summary>
/// <remarks>
/// <c>ILogger</c> is fully qualified: this project globally imports Serilog, which declares
/// a type of the same name.
/// </remarks>
internal static partial class ExceptionLog
{
    [LoggerMessage(
        EventId = 4000,
        Level = LogLevel.Error,
        Message = "An unhandled exception occurred while processing {Path}.")]
    public static partial void UnhandledException(
        this Microsoft.Extensions.Logging.ILogger logger,
        string path,
        Exception exception);
}
