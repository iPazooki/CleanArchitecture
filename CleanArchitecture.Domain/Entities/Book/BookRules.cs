namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// Defines shared validation rules for books.
/// </summary>
public static class BookRules
{
    /// <summary>
    /// Gets the minimum allowed title length.
    /// </summary>
    public const int TitleMinLength = 3;

    /// <summary>
    /// Gets the maximum allowed title length.
    /// </summary>
    public const int TitleMaxLength = 200;

    /// <summary>
    /// Gets the minimum allowed genre length.
    /// </summary>
    public const int GenreMinLength = 1;

    /// <summary>
    /// Gets the maximum allowed genre length.
    /// </summary>
    public const int GenreMaxLength = 2;
}
