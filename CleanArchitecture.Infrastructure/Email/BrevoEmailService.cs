using System.Net.Http.Json;
using System.Text.Json;
using CleanArchitecture.Application.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace CleanArchitecture.Infrastructure.Email;

/// <summary>
/// Sends transactional email through the Brevo REST API (<c>POST /v3/smtp/email</c>).
/// </summary>
/// <remarks>
/// Transport faults, rate limits and 5xx responses are handled by the standard resilience
/// pipeline attached to the injected <see cref="HttpClient"/>. Once the pipeline gives up,
/// the failure is expected rather than exceptional, so it surfaces as a failed
/// <see cref="Result"/> instead of an exception.
/// </remarks>
internal sealed class BrevoEmailService(
    HttpClient httpClient,
    IOptions<BrevoOptions> options,
    ILogger<BrevoEmailService> logger) : IEmailService
{
    private const string SendEndpoint = "smtp/email";

    /// <inheritdoc />
    /// <param name="receiver">The recipient's email address.</param>
    /// <param name="subject">The subject line.</param>
    /// <param name="body">The HTML body of the message.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task<Result> SendAsync(
        string receiver,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(receiver);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        BrevoOptions brevo = options.Value;

        BrevoEmailRequest request = new(
            new BrevoSender(brevo.SenderName, brevo.SenderEmail),
            [new BrevoRecipient(receiver)],
            subject,
            body);

        try
        {
            using HttpResponseMessage response = await httpClient
                .PostAsJsonAsync(SendEndpoint, request, BrevoJsonSerializerContext.Default.BrevoEmailRequest, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                await LogRejectionAsync(response, receiver, cancellationToken).ConfigureAwait(false);
                return Result.Failure(EmailErrors.SendFailed);
            }

            BrevoEmailResponse? accepted = await response.Content
                .ReadFromJsonAsync(BrevoJsonSerializerContext.Default.BrevoEmailResponse, cancellationToken)
                .ConfigureAwait(false);

            logger.EmailSent(receiver, accepted?.MessageId ?? "unknown");

            return Result.Success();
        }
        catch (HttpRequestException exception)
        {
            logger.EmailTransportFailed(receiver, exception);
            return Result.Failure(EmailErrors.SendFailed);
        }
        catch (ExecutionRejectedException exception)
        {
            // The resilience pipeline gave up: circuit open, or the attempt/total timeout elapsed.
            logger.EmailTransportFailed(receiver, exception);
            return Result.Failure(EmailErrors.SendFailed);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            // HttpClient surfaces its own timeout as a cancellation the caller never asked for.
            logger.EmailTransportFailed(receiver, exception);
            return Result.Failure(EmailErrors.SendFailed);
        }
    }

    private async Task LogRejectionAsync(
        HttpResponseMessage response,
        string receiver,
        CancellationToken cancellationToken)
    {
        BrevoErrorResponse? error = null;

        try
        {
            error = await response.Content
                .ReadFromJsonAsync(BrevoJsonSerializerContext.Default.BrevoErrorResponse, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (JsonException)
        {
            // Brevo does not guarantee a JSON body on every failure; the status code is enough.
        }
        catch (NotSupportedException)
        {
            // Unexpected content type. Same reasoning.
        }

        logger.EmailRejected(receiver, (int)response.StatusCode, error?.Code, error?.Message);
    }
}
