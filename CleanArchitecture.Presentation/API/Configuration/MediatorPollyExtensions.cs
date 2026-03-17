namespace CleanArchitecture.Api.Configuration;

internal static class MediatorPollyExtensions
{
    private static readonly IAsyncPolicy<Result> _retryPolicy =
        Policy<Result>
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));

    private static readonly IAsyncPolicy<Result> _circuitBreakerPolicy =
        Policy<Result>
            .Handle<Exception>()
            .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));

    private static readonly IAsyncPolicy<Result> _fallbackPolicy =
        Policy<Result>
            .Handle<Exception>()
            .FallbackAsync(Result.Failure("API call is not successful."));

    private static readonly Polly.Wrap.AsyncPolicyWrap<Result> _policyWrap =
        _fallbackPolicy
            .WrapAsync(_circuitBreakerPolicy)
            .WrapAsync(_retryPolicy);

    public static async ValueTask<Result> SendWithRetryAsync(this ISender sender, IRequest<Result> request)
    {
        return await _policyWrap.ExecuteAsync(async () => await sender.Send(request).ConfigureAwait(false)).ConfigureAwait(false);
    }
}
