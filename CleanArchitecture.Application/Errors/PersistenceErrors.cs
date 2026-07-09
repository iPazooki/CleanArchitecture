using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Application.Errors;

/// <summary>
/// Errors returned when a unit of work fails to commit.
/// </summary>
/// <remarks>
/// The messages are deliberately opaque. A database exception's own message names tables,
/// columns and constraints — sometimes values — and must never reach an HTTP response.
/// The exception itself is logged at the point of failure instead.
/// </remarks>
public static class PersistenceErrors
{
    /// <summary>The write could not be committed.</summary>
    public static DomainError SaveFailed => DomainError.Failure(
        "Persistence.SaveFailed",
        "The changes could not be saved.");

    /// <summary>Another transaction modified the same rows first.</summary>
    public static DomainError ConcurrencyConflict => DomainError.Conflict(
        "Persistence.ConcurrencyConflict",
        "The record was modified by another operation. Reload it and try again.");
}
