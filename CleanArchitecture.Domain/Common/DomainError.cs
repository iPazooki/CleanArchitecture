namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Represents a domain error with a code, message, and classifying type.
/// </summary>
/// <remarks>
/// Inherits <see cref="Error"/> rather than converting to it, so <see cref="Type"/> survives
/// into a <see cref="Result"/> and the Presentation layer can map it to a status code by
/// pattern matching instead of guessing from <see cref="Error.Code"/>.
/// </remarks>
/// <param name="Code">The unique error code.</param>
/// <param name="Message">The human-readable error message.</param>
/// <param name="Type">The classification of the error (see <see cref="ErrorType"/>).</param>
public sealed record DomainError(string Code, string Message, ErrorType Type = ErrorType.Failure)
    : Error(Message, Code)
{
    /// <summary>
    /// Represents the absence of an error.
    /// </summary>
    public static readonly DomainError None = new(string.Empty, string.Empty, ErrorType.None);

    /// <summary>
    /// Creates a validation error.
    /// </summary>
    public static DomainError Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    /// <summary>
    /// Creates a not found error.
    /// </summary>
    public static DomainError NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    /// <summary>
    /// Creates a conflict error.
    /// </summary>
    public static DomainError Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    /// <summary>
    /// Creates a failure error.
    /// </summary>
    public static DomainError Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);
}
