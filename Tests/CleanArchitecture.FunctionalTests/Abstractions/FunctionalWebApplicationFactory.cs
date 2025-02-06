using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using CleanArchitecture.Application.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CleanArchitecture.FunctionalTests.Abstractions;

/// <summary>
/// A custom WebApplicationFactory for functional tests.
/// </summary>
/// <param name="serviceCollection">An action to configure the service collection.</param>
internal class FunctionalWebApplicationFactory(Action<IServiceCollection> serviceCollection)
    : WebApplicationFactory<Program>
{
    /// <summary>
    /// Creates and configures the host for the test server.
    /// </summary>
    /// <param name="builder">The host builder.</param>
    /// <returns>The configured host.</returns>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(serviceCollection);

        return base.CreateHost(builder);
    }

    /// <summary>
    /// Configures the web host for the test server.
    /// </summary>
    /// <param name="builder">The web host builder.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set the environment to Testing
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        builder.ConfigureTestServices(services =>
        {
            // Remove all instances of IApplicationUnitOfWork from the service collection
            services.RemoveAll<IApplicationUnitOfWork>();
        });
    }
}