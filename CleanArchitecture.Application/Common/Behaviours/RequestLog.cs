namespace CleanArchitecture.Application.Common.Behaviours;

/// <summary>
/// Source-generated log messages for the mediator pipeline.
/// </summary>
internal static partial class RequestLog
{
    [LoggerMessage(
        EventId = 2000,
        Level = LogLevel.Information,
        Message = "Handling request {RequestName}.")]
    public static partial void HandlingRequest(this ILogger logger, string requestName);

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Handled request {RequestName} in {ElapsedMilliseconds}ms.")]
    public static partial void RequestHandled(this ILogger logger, string requestName, long elapsedMilliseconds);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Warning,
        Message = "Request {RequestName} took {ElapsedMilliseconds}ms, exceeding the {ThresholdMilliseconds}ms threshold.")]
    public static partial void SlowRequest(
        this ILogger logger,
        string requestName,
        long elapsedMilliseconds,
        int thresholdMilliseconds);

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Error,
        Message = "Request {RequestName} failed after {ElapsedMilliseconds}ms: {Errors}")]
    public static partial void RequestFailed(
        this ILogger logger,
        string requestName,
        long elapsedMilliseconds,
        string errors);
}
