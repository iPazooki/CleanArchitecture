namespace CleanArchitecture.Domain.Validations.Book;

/// <summary>
/// Defines shared validation rules for books. These constants are the single source
/// of truth for the Book aggregate invariants and the Application-layer validators.
/// </summary>
public static class BookRules
{
    /// <summary>
    /// The minimum allowed title length.
    /// </summary>
    public const int TitleMinLength = 3;

    /// <summary>
    /// The maximum allowed title length.
    /// </summary>
    public const int TitleMaxLength = 200;

    /// <summary>
    /// The minimum allowed genre code length.
    /// </summary>
    public const int GenreMinLength = 1;

    /// <summary>
    /// The maximum allowed genre code length.
    /// </summary>
    public const int GenreMaxLength = 2;
}
