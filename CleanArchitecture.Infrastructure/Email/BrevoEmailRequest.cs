namespace CleanArchitecture.Infrastructure.Email;

/// <summary>
/// Body of <c>POST /v3/smtp/email</c>.
/// </summary>
/// <param name="Sender">The verified sender the message originates from.</param>
/// <param name="To">The recipients of the message.</param>
/// <param name="Subject">The subject line.</param>
/// <param name="HtmlContent">The HTML body of the message.</param>
internal sealed record BrevoEmailRequest(
    BrevoSender Sender,
    IReadOnlyList<BrevoRecipient> To,
    string Subject,
    string HtmlContent);

/// <summary>The <c>sender</c> object of a Brevo transactional email request.</summary>
internal sealed record BrevoSender(string Name, string Email);

/// <summary>An entry of the <c>to</c> array of a Brevo transactional email request.</summary>
internal sealed record BrevoRecipient(string Email);
