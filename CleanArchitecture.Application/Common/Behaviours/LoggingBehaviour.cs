using System.Diagnostics;

namespace CleanArchitecture.Application.Common.Behaviours;

/// <summary>
/// Logs the outcome and duration of every request flowing through the mediator pipeline.
/// </summary>
/// <remarks>
/// Registered outermost, so it also observes the failures short-circuited by
/// <see cref="ValidationBehaviour{TRequest, TResponse}"/>.
/// </remarks>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type, a <see cref="Result"/> or <see cref="Result{TValue}"/>.</typeparam>
internal sealed class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private const int SlowRequestThresholdMilliseconds = 1000;

    private static readonly string _requestName = typeof(TRequest).Name;

    public async ValueTask<TResponse> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        logger.HandlingRequest(_requestName);

        long startTimestamp = Stopwatch.GetTimestamp();

        TResponse result = await next(message, cancellationToken).ConfigureAwait(false);

        long elapsedMilliseconds = (long)Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

        if (elapsedMilliseconds > SlowRequestThresholdMilliseconds)
        {
            logger.SlowRequest(_requestName, elapsedMilliseconds, SlowRequestThresholdMilliseconds);
        }

        if (result.IsSuccess)
        {
            logger.RequestHandled(_requestName, elapsedMilliseconds);
        }
        else
        {
            logger.RequestFailed(_requestName, elapsedMilliseconds, FormatErrors(result.Errors));
        }

        return result;
    }

    private static string FormatErrors(IEnumerable<Error> errors) =>
        string.Join("; ", errors.Select(error => $"{error.Code ?? "Error"}: {error.Message}"));
}
