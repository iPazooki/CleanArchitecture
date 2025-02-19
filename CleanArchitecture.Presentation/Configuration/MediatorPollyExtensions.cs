namespace CleanArchitecture.Presentation.Configuration;

public static class MediatorPollyExtensions
{
    private static readonly IAsyncPolicy<Result> RetryPolicy =
        Policy<Result>
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));

    private static readonly IAsyncPolicy<Result> CircuitBreakerPolicy =
        Policy<Result>
            .Handle<Exception>()
            .CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));

    private static readonly IAsyncPolicy<Result> FallbackPolicy =
        Policy<Result>
            .Handle<Exception>()
            .FallbackAsync(Result.Failure("API call is not successful."));

    private static readonly IAsyncPolicy<Result> PolicyWrap =
        FallbackPolicy
            .WrapAsync(CircuitBreakerPolicy)
            .WrapAsync(RetryPolicy);

    public static Task<Result> SendWithRetryAsync(this ISender sender, IRequest<Result> request) =>
        PolicyWrap.ExecuteAsync(() => sender.Send(request));
}