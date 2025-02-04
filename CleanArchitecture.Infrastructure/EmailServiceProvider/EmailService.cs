namespace CleanArchitecture.Infrastructure.EmailServiceProvider;

/// <summary>
/// Provides email sending functionality.
/// </summary>
public class EmailService : IEmailService
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="receiver">The email address of the receiver.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body content of the email.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the operation.</returns>
    public Task<Result> SendAsync(string receiver, string subject, string body)
    {
        throw new NotImplementedException();
    }
}