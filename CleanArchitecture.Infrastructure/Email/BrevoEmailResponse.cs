namespace CleanArchitecture.Infrastructure.Email;

/// <summary>
/// Successful response of <c>POST /v3/smtp/email</c>.
/// </summary>
/// <param name="MessageId">Identifier of the accepted message, used to correlate deliverability events.</param>
internal sealed record BrevoEmailResponse(string? MessageId);
