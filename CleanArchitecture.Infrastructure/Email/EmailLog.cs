using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Email;

/// <summary>
/// Source-generated log messages for the email integration.
/// </summary>
/// <remarks>
/// The Brevo API key must never be logged; only the recipient and the returned message id are.
/// </remarks>
internal static partial class EmailLog
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "Email accepted by Brevo for {Receiver} with message id {MessageId}.")]
    public static partial void EmailSent(this ILogger logger, string receiver, string messageId);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Error,
        Message = "Brevo rejected the email for {Receiver} with status {StatusCode}: {ErrorCode} {ErrorMessage}.")]
    public static partial void EmailRejected(
        this ILogger logger,
        string receiver,
        int statusCode,
        string? errorCode,
        string? errorMessage);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Error,
        Message = "Could not reach Brevo to send an email to {Receiver}.")]
    public static partial void EmailTransportFailed(this ILogger logger, string receiver, Exception exception);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Warning,
        Message = "No email provider is configured; dropping the email addressed to {Receiver}.")]
    public static partial void EmailProviderNotConfigured(this ILogger logger, string receiver);
}
