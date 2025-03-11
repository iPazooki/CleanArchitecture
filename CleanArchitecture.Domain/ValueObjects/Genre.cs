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
    /// <returns>A Genre instance.</returns>
    /// <exception cref="UnsupportedGenreException">Thrown when the genre is not supported.</exception>
    public static Genre FromCode(string code)
    {
        Genre genre = new(code);

        if (!SupportedGenres.Contains(genre))
        {
            throw new UnsupportedGenreException(code);
        }

        return genre;
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
    /// Explicitly converts a string to a Genre.
    /// </summary>
    /// <param name="code">The code to convert.</param>
    public static explicit operator Genre(string code) => FromCode(code);

    /// <summary>
    /// Returns the string representation of the Genre.
    /// </summary>
    /// <returns>The code of the genre.</returns>
    public override string ToString() => Code;
}
