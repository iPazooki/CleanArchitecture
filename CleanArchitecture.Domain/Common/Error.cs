namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Represents a domain error with code, message, and type information.
/// </summary>
/// <param name="Code">The unique error code.</param>
/// <param name="Message">The human-readable error message.</param>
/// <param name="Type">The type of error (Validation, NotFound, Conflict, Failure).</param>
public sealed record DomainError(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
    /// <summary>
    /// Represents a successful result with no error.
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
    /// Implicit conversion from DomainError to string for backward compatibility.
    /// </summary>
    public static implicit operator string(DomainError error) => error?.Code ?? string.Empty;
}

/// <summary>
/// Defines the types of errors that can occur.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// No error.
    /// </summary>
    None = 0,

    /// <summary>
    /// Validation error (HTTP 422).
    /// </summary>
    Validation = 1,

    /// <summary>
    /// Resource not found (HTTP 404).
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// Conflict error (HTTP 409).
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// General failure (HTTP 400 or 500).
    /// </summary>
    Failure = 4
}
