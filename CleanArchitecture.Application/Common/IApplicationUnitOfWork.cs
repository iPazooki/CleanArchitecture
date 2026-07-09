namespace CleanArchitecture.Application.Common;

/// <summary>
/// Represents an application-specific unit of work that exposes the aggregate DbSets and persists changes.
/// Lifetime is managed by the DI container (scoped); do not manually dispose.
/// </summary>
public interface IApplicationUnitOfWork
{
    /// <summary>
    /// Gets the DbSet for <see cref="Book"/> entities.
    /// </summary>
    public DbSet<Book> Books { get; }

    /// <summary>
    /// Saves all changes made in this unit of work asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains a <see cref="Result"/>.</returns>
    public Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}