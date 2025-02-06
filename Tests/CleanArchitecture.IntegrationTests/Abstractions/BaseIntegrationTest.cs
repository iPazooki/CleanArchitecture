using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.IntegrationTests.Abstractions;

/// <summary>
/// Base class for integration tests, providing common setup and teardown functionality.
/// Implements <see cref="IClassFixture{TFixture}"/> to share a single instance of <see cref="IntegrationWebApplicationFactory"/> across tests.
/// </summary>
public abstract class BaseIntegrationTest : IClassFixture<IntegrationWebApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseIntegrationTest"/> class.
    /// Creates a new service scope and retrieves the <see cref="ISender"/> service.
    /// </summary>
    protected BaseIntegrationTest()
    {
        IntegrationWebApplicationFactory factory = new();

        _scope = factory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
    }

    /// <summary>
    /// Gets the <see cref="ISender"/> service used to send commands and queries.
    /// </summary>
    protected ISender Sender { get; }

    /// <summary>
    /// Disposes the service scope.
    /// </summary>
    public void Dispose() => _scope.Dispose();
}