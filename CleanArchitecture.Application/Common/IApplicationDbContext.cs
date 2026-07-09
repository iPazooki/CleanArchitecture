namespace CleanArchitecture.Application.Common;

/// <summary>
/// The application's view of the database: the aggregate roots it may query, and a way to
/// persist pending changes. Lifetime is managed by the DI container (scoped); do not manually dispose.
/// </summary>
/// <remarks>
/// This is a <see cref="DbContext"/> facade, not a Unit of Work — it is named for what it is.
/// The Application layer knowingly depends on EF Core so queries can be composed and projected
/// server-side; hiding <see cref="DbSet{TEntity}"/> behind a repository here would trade one
/// coupling for a leakier one.
/// </remarks>
public interface IApplicationDbContext
{
    /// <summary>
    /// Gets the DbSet for <see cref="Book"/> entities.
    /// </summary>
    DbSet<Book> Books { get; }

    /// <summary>
    /// Persists all pending changes.
    /// </summary>
    /// <remarks>
    /// Named <c>SaveEntitiesAsync</c> rather than <c>SaveChangesAsync</c> because
    /// <see cref="DbContext"/> implements this interface directly, and a
    /// <c>Task&lt;Result&gt; SaveChangesAsync(CancellationToken)</c> would collide with EF Core's
    /// <c>Task&lt;int&gt; SaveChangesAsync(CancellationToken)</c>.
    /// </remarks>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A successful <see cref="Result"/>, or a failure describing why the write did not commit.</returns>
    Task<Result> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}
