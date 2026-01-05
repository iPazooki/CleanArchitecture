using Aspire.Hosting;

namespace CleanArchitecture.IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest(DistributedApplicationFixture fixture)
{
    protected DistributedApplication App => fixture._app;

    protected TimeSpan DefaultTimeout => fixture._defaultTimeout;

    protected CancellationToken CancellationToken => fixture._cancellationToken;
}
