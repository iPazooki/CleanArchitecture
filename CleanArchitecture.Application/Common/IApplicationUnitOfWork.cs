using CleanArchitecture.Domain.Entities.Book;
using CleanArchitecture.Domain.Entities.Order;
using CleanArchitecture.Domain.Entities.Person;

namespace CleanArchitecture.Application.Common;

/// <summary>
/// Represents a unit of work that manages the persistence of changes.
/// </summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Saves all changes made in this unit of work asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains a <see cref="Result"/>.</returns>
    public Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an application-specific unit of work that includes specific DbSets.
/// </summary>
public interface IApplicationUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Gets the DbSet for <see cref="Person"/> entities.
    /// </summary>
    public DbSet<Person> Persons { get; }

    /// <summary>
    /// Gets the DbSet for <see cref="Book"/> entities.
    /// </summary>
    public DbSet<Book> Books { get; }

    public DbSet<Order> Orders { get; }
}