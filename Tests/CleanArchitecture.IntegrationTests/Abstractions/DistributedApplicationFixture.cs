using Aspire.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.IntegrationTests.Abstractions;

public class DistributedApplicationFixture : IAsyncLifetime
{
    internal readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes(1);

    internal DistributedApplication _app = null!;

    internal CancellationToken _cancellationToken = CancellationToken.None;

    public async ValueTask InitializeAsync()
    {
        IDistributedApplicationTestingBuilder appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.CleanArchitecture_AppHost>(["--environment=Testing"], _cancellationToken);
        
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Warning);
        });

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _app = await appHost.BuildAsync(_cancellationToken).WaitAsync(_defaultTimeout, _cancellationToken);

        await _app.StartAsync(_cancellationToken).WaitAsync(_defaultTimeout, _cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
