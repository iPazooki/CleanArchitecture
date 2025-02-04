namespace CleanArchitecture.Domain.Exceptions;

/// <summary>
/// Exception thrown when an unsupported genre code is encountered.
/// </summary>
/// <param name="code">The unsupported genre code.</param>
public sealed class UnsupportedGenreException(string code)
    : Exception($"Genre \"{code}\" is unsupported.");