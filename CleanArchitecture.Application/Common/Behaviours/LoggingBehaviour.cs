using System.Diagnostics;

namespace CleanArchitecture.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private const int SlowRequestThresholdMilliseconds = 1000;

    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        Stopwatch stopWatch = Stopwatch.StartNew();

        logger.LogInformation("Handling request {Name}, {DateTime} ", message.GetType().Name, DateTimeOffset.UtcNow);

        ArgumentNullException.ThrowIfNull(next);

        TResponse result = await next(message, cancellationToken).ConfigureAwait(false);

        stopWatch.Stop();

        if (stopWatch.ElapsedMilliseconds > SlowRequestThresholdMilliseconds)
        {
            logger.LogWarning("Request took too long to handle {Name}, {DateTime}, {ElapsedMilliseconds}ms", message.GetType().Name, DateTimeOffset.UtcNow, stopWatch.ElapsedMilliseconds);
        }

        if (result.IsSuccess)
        {
            logger.LogInformation("Request handled successfully {Name}, {DateTime}", message.GetType().Name, DateTimeOffset.UtcNow);
        }
        else
        {
            logger.LogError("Request failed to handle {Name}, {DateTime}, {@Error}", message.GetType().Name, DateTimeOffset.UtcNow, result.Errors);
        }

        return result;
    }
}
