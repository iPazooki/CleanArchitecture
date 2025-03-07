using System.Diagnostics;

namespace CleanArchitecture.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private const int SlowRequestThresholdMilliseconds = 1000;
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Stopwatch stopWatch = Stopwatch.StartNew();
        
        logger.LogInformation("Handling request {@Name}, {@DateTime} ", request.GetType().Name, DateTime.UtcNow);

        TResponse result = await next();
        
        stopWatch.Stop();
        
        if (stopWatch.ElapsedMilliseconds > SlowRequestThresholdMilliseconds)
        {
            logger.LogWarning("Request took too long to handle {@Name}, {@DateTime}, {@ElapsedMilliseconds}ms", request.GetType().Name, DateTime.UtcNow, stopWatch.ElapsedMilliseconds);
        }
        
        if (result.IsSuccess)
        {
            logger.LogInformation("Request handled successfully {@Name}, {@DateTime}, {@Error}", request.GetType().Name, DateTime.UtcNow, result.Errors);
        }
        else
        {
            logger.LogError("Request failed to handle {@Name}, {@DateTime}, {@Error}", request.GetType().Name, DateTime.UtcNow, result.Errors);
        }
        
        return result;
    }
}