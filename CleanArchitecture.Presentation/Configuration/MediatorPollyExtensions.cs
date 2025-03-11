namespace CleanArchitecture.Presentation.Configuration;

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

    private static readonly Polly.Wrap.AsyncPolicyWrap<DomainValidation.Result> _policyWrap =
        _fallbackPolicy
            .WrapAsync(_circuitBreakerPolicy)
            .WrapAsync(_retryPolicy);

    public static Task<Result> SendWithRetryAsync(this ISender sender, IRequest<Result> request) =>
        _policyWrap.ExecuteAsync(() => sender.Send(request));
}
