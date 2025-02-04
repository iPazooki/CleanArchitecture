namespace CleanArchitecture.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for an email service that can send emails asynchronously.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="receiver">The email address of the receiver.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body content of the email.</param>
    /// <returns>A task that represents the asynchronous send operation. The task result contains a <see cref="Result"/> indicating the outcome of the operation.</returns>
    Task<Result> SendAsync(string receiver, string subject, string body);
}