using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Infrastructure.Email;

/// <summary>
/// Configuration for the Brevo transactional email API.
/// </summary>
/// <remarks>
/// Bound from the <c>Brevo</c> configuration section and validated at startup, so a
/// misconfigured deployment fails fast rather than on the first attempt to send.
/// <see cref="ApiKey"/> is a secret: supply it through user-secrets locally and through
/// Key Vault (via the Aspire <c>brevo-api-key</c> parameter) in production. Never commit it.
/// </remarks>
internal sealed class BrevoOptions
{
    /// <summary>The configuration section this type binds to.</summary>
    public const string SectionName = "Brevo";

    /// <summary>The Brevo API key, sent in the <c>api-key</c> request header.</summary>
    [Required(AllowEmptyStrings = false)]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>The display name messages are sent from.</summary>
    [Required(AllowEmptyStrings = false)]
    public string SenderName { get; set; } = string.Empty;

    /// <summary>The address messages are sent from. Must be a verified Brevo sender.</summary>
    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>Root of the Brevo REST API. The trailing slash is required for relative request URIs to resolve.</summary>
    [Required(AllowEmptyStrings = false)]
    [Url]
    public string BaseAddress { get; set; } = "https://api.brevo.com/v3/";
}
