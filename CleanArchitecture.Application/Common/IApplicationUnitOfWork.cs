using CleanArchitecture.Domain.Entities.Book;
using CleanArchitecture.Domain.Entities.Order;
using CleanArchitecture.Domain.Entities.Security;
using User = CleanArchitecture.Domain.Entities.User.User;

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
    /// Gets the DbSet for <see cref="User"/> entities.
    /// </summary>
    public DbSet<User> Users { get; }

    /// <summary>
    /// Gets the DbSet for <see cref="Book"/> entities.
    /// </summary>
    public DbSet<Book> Books { get; }

    /// <summary>
    /// Gets the DbSet for <see cref="Order"/> entities.
    /// </summary>
    public DbSet<Order> Orders { get; }
    
    /// <summary>
    /// Gets the DbSet for <see cref="Role"/> entities.
    /// </summary>
    public DbSet<Role> Roles { get; }
    
    /// <summary>
    /// Gets the DbSet for <see cref="Permission"/> entities.
    /// </summary>
    public DbSet<Permission> Permissions { get; }
}