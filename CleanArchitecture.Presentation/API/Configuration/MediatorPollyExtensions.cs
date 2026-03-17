namespace CleanArchitecture.Api.Configuration;

internal static class MediatorPollyExtensions
{
    private const string ApiCallFailureMessage = "API call is not successful.";

    public static async ValueTask<Result> SendWithRetryAsync(this ISender sender, IRequest<Result> request)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(request);

        Polly.Wrap.AsyncPolicyWrap<Result> policyWrap = CreatePolicyWrap(Result.Failure(ApiCallFailureMessage));

        return await policyWrap.ExecuteAsync(async () => await sender.Send(request).ConfigureAwait(false)).ConfigureAwait(false);
    }

    public static async ValueTask<Result<TResponse>> SendWithRetryAsync<TResponse>(this ISender sender, IRequest<Result<TResponse>> request)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(request);

        Polly.Wrap.AsyncPolicyWrap<Result<TResponse>> policyWrap = CreatePolicyWrap(Result<TResponse>.Failure(ApiCallFailureMessage));

        return await policyWrap.ExecuteAsync(async () => await sender.Send(request).ConfigureAwait(false)).ConfigureAwait(false);
    }

    private static Polly.Wrap.AsyncPolicyWrap<TResponse> CreatePolicyWrap<TResponse>(TResponse fallbackResult)
    {
        IAsyncPolicy<TResponse> retryPolicy = Policy<TResponse>
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));

        IAsyncPolicy<TResponse> circuitBreakerPolicy = Policy<TResponse>
            .Handle<Exception>()
            .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));

        IAsyncPolicy<TResponse> fallbackPolicy = Policy<TResponse>
            .Handle<Exception>()
            .FallbackAsync(fallbackResult);

        return fallbackPolicy
            .WrapAsync(circuitBreakerPolicy)
            .WrapAsync(retryPolicy);
    }
}
