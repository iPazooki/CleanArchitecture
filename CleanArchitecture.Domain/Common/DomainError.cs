namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Represents a domain error with a code, message, and classifying type.
/// </summary>
/// <param name="Code">The unique error code.</param>
/// <param name="Message">The human-readable error message.</param>
/// <param name="Type">The classification of the error (see <see cref="ErrorType"/>).</param>
public sealed record DomainError(string Code, string Message, ErrorType Type = ErrorType.Failure)
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

    /// <summary>
    /// Implicit conversion from <see cref="DomainError"/> to <see cref="Error"/>.
    /// </summary>
    public static implicit operator Error(DomainError error) => new(Message: error.Message, Code: error.Code);
}
