namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Classifies the nature of a <see cref="DomainError"/>. How each type maps to a
/// transport-level response (e.g. an HTTP status code) is a Presentation-layer concern.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// No error.
    /// </summary>
    None = 0,

    /// <summary>
    /// A validation rule was violated.
    /// </summary>
    Validation = 1,

    /// <summary>
    /// The requested resource was not found.
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// The operation conflicts with the current state.
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// A general, unclassified failure.
    /// </summary>
    Failure = 4
}
