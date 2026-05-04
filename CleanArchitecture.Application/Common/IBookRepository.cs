namespace CleanArchitecture.Application.Common;

/// <summary>
/// Defines the contract for a repository that manages book entities.
/// </summary>
public interface IBookRepository
{
    /// <summary>
    /// Retrieves a book by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the book to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the book if found; otherwise, null.</returns>
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of books in the repository.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the total number of books.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of books.
    /// </summary>
    /// <param name="skip">The number of records to skip before starting to yield results.</param>
    /// <param name="take">The maximum number of records to return.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of books.</returns>
    Task<IReadOnlyList<Book>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new book to the repository.
    /// </summary>
    /// <param name="book">The book entity to add.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(Book book, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an existing book from the repository.
    /// </summary>
    /// <param name="book">The book entity to remove.</param>
    void Remove(Book book);
}
