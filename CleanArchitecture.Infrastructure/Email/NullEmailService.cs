using CleanArchitecture.Application.Errors;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Email;

/// <summary>
/// The <see cref="IEmailService"/> used when no email provider is configured.
/// </summary>
/// <remarks>
/// Registered in place of <see cref="BrevoEmailService"/> when <c>Brevo:ApiKey</c> is absent.
/// It keeps <see cref="IEmailService"/> resolvable — so the host still starts and nothing throws
/// — while making the dropped message loud in the logs and visible to the caller as a failed
/// <see cref="Result"/>.
/// </remarks>
internal sealed class NullEmailService(ILogger<NullEmailService> logger) : IEmailService
{
    /// <inheritdoc />
    public Task<Result> SendAsync(
        string receiver,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(receiver);

        logger.EmailProviderNotConfigured(receiver);

        return Task.FromResult(Result.Failure(EmailErrors.NotConfigured));
    }
}
