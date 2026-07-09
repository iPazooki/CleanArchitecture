namespace CleanArchitecture.Infrastructure.Email;

/// <summary>
/// Brevo's <c>errorModel</c>, returned on a failed request.
/// </summary>
/// <param name="Code">Machine-readable failure code.</param>
/// <param name="Message">Human-readable description of the failure.</param>
internal sealed record BrevoErrorResponse(string? Code, string? Message);
