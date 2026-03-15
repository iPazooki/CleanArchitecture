namespace CleanArchitecture.Domain.ValueObjects;

/// <summary>
/// Represents a genre with a code.
/// </summary>
/// <param name="Code">The code of the genre.</param>
public sealed record Genre(string Code)
{
    /// <summary>
    /// Creates a Genre instance from a code.
    /// </summary>
    /// <param name="code">The code of the genre.</param>
    /// <returns>A Result containing the Genre or a validation error.</returns>
    public static Result<Genre> FromCode(string code)
    {
        Genre genre = new(code);

        if (!SupportedGenres.Contains(genre))
        {
            return Result<Genre>.Failure(BookErrors.InvalidGenre);
        }

        return Result<Genre>.Success(genre);
    }

    /// <summary>
    /// Represents the Fiction genre.
    /// </summary>
    public static Genre Fiction => new("F");

    /// <summary>
    /// Represents the Non-Fiction genre.
    /// </summary>
    public static Genre NonFiction => new("NF");

    /// <summary>
    /// Represents the Mystery genre.
    /// </summary>
    public static Genre Mystery => new("M");

    /// <summary>
    /// Gets the supported genres.
    /// </summary>
    private static IEnumerable<Genre> SupportedGenres
    {
        get
        {
            yield return Fiction;
            yield return NonFiction;
            yield return Mystery;
        }
    }

    /// <summary>
    /// Implicitly converts a Genre to a string.
    /// </summary>
    /// <param name="genre">The Genre to convert.</param>
    public static implicit operator string(Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre);

        return genre.ToString();
    }

    /// <summary>
    /// Returns the string representation of the Genre.
    /// </summary>
    /// <returns>The code of the genre.</returns>
    public override string ToString() => Code;
}
