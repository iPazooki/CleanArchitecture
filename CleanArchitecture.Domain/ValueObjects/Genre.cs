using CleanArchitecture.Domain.Validations;

namespace CleanArchitecture.Domain.ValueObjects;

/// <summary>
/// Represents a book genre identified by a canonical code.
/// </summary>
/// <param name="Code">The canonical code of the genre.</param>
public sealed record Genre(string Code)
{
    /// <summary>
    /// The Fiction genre.
    /// </summary>
    public static Genre Fiction { get; } = new("F");

    /// <summary>
    /// The Non-Fiction genre.
    /// </summary>
    public static Genre NonFiction { get; } = new("NF");

    /// <summary>
    /// The Mystery genre.
    /// </summary>
    public static Genre Mystery { get; } = new("M");

    /// <summary>
    /// Gets the supported genres as a frozen set for O(1) lookup.
    /// </summary>
    private static FrozenSet<Genre> SupportedGenres { get; } = new HashSet<Genre>
    {
        Fiction,
        NonFiction,
        Mystery
    }.ToFrozenSet();

    /// <summary>
    /// Creates a <see cref="Genre"/> from a code, validating that it is supported.
    /// </summary>
    /// <param name="code">The genre code (surrounding whitespace is ignored).</param>
    /// <returns>A <see cref="Result{T}"/> containing the genre or a validation error.</returns>
    public static Result<Genre> FromCode(string? code)
    {
        Genre genre = new((code ?? string.Empty).Trim());

        return SupportedGenres.Contains(genre)
            ? Result<Genre>.Success(genre)
            : Result<Genre>.Failure(BookErrors.InvalidGenre);
    }

    /// <summary>
    /// Returns the canonical code of the genre.
    /// </summary>
    /// <returns>The genre code.</returns>
    public override string ToString() => Code;
}
