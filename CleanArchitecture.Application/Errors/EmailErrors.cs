using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Application.Errors;

/// <summary>
/// Contains the errors returned by <see cref="Common.Interfaces.IEmailService"/> implementations.
/// </summary>
public static class EmailErrors
{
    /// <summary>The email provider rejected the message or was unreachable.</summary>
    public static DomainError SendFailed => DomainError.Failure(
        "Email.SendFailed",
        "The email could not be sent.");

    /// <summary>No email provider is configured for the current environment.</summary>
    public static DomainError NotConfigured => DomainError.Failure(
        "Email.NotConfigured",
        "No email provider is configured.");
}
