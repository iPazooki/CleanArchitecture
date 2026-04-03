namespace CleanArchitecture.Application.Common;

/// <summary>
/// Represents a paginated response with metadata.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
/// <param name="Items">The items for the current page.</param>
/// <param name="Page">The current page number (1-based).</param>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="TotalCount">The total number of items across all pages.</param>
public record PaginatedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}