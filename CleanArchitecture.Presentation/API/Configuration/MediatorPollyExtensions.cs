using Polly.Retry;
using Polly.CircuitBreaker;
using Polly.Fallback;

namespace CleanArchitecture.Api.Configuration;

internal static class MediatorPollyExtensions
{
    private const string ApiCallFailureMessage = "API call is not successful.";
    private const int MaxRetryAttempts = 3;
    private const int CircuitBreakerFailureThreshold = 2;

    public static async ValueTask<Result> SendWithRetryAsync(this ISender sender, IRequest<Result> request)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(request);

        ResiliencePipeline<Result> pipeline = CreateResiliencePipeline(Result.Failure(ApiCallFailureMessage));

        return await pipeline.ExecuteAsync(
            async ct => await sender.Send(request, ct).ConfigureAwait(false),
            CancellationToken.None).ConfigureAwait(false);
    }

    public static async ValueTask<Result<TResponse>> SendWithRetryAsync<TResponse>(this ISender sender, IRequest<Result<TResponse>> request)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(request);

        ResiliencePipeline<Result<TResponse>> pipeline = CreateResiliencePipeline(Result<TResponse>.Failure(ApiCallFailureMessage));

        return await pipeline.ExecuteAsync(
            async ct => await sender.Send(request, ct).ConfigureAwait(false),
            CancellationToken.None).ConfigureAwait(false);
    }

    private static ResiliencePipeline<TResponse> CreateResiliencePipeline<TResponse>(TResponse fallbackResult)
    {
        return new ResiliencePipelineBuilder<TResponse>()
            .AddFallback(new FallbackStrategyOptions<TResponse>
            {
                ShouldHandle = new PredicateBuilder<TResponse>()
                    .HandleTransientExceptions(),
                FallbackAction = _ => Outcome.FromResultAsValueTask(fallbackResult)
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<TResponse>
            {
                ShouldHandle = new PredicateBuilder<TResponse>()
                    .HandleTransientExceptions(),
                FailureRatio = 0.5,
                MinimumThroughput = CircuitBreakerFailureThreshold,
                BreakDuration = TimeSpan.FromMinutes(1)
            })
            .AddRetry(new RetryStrategyOptions<TResponse>
            {
                ShouldHandle = new PredicateBuilder<TResponse>()
                    .HandleTransientExceptions(),
                MaxRetryAttempts = MaxRetryAttempts,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential
            })
            .Build();
    }

    private static PredicateBuilder<TResponse> HandleTransientExceptions<TResponse>(this PredicateBuilder<TResponse> builder)
    {
        return builder
            .Handle<TimeoutException>()
            .Handle<HttpRequestException>()
            .Handle<Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException>();
    }
}