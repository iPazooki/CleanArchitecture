using DomainValidation;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Persistence.Data.UnitOfWork;

/// <summary>
/// Represents the unit of work for the application.
/// </summary>
/// <param name="context">The application database context.</param>
public sealed partial class ApplicationUnitOfWork(ApplicationDbContext context, ILogger<ApplicationUnitOfWork> logger) : IApplicationUnitOfWork
{
    /// <summary>
    /// Saves the changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the operation.</returns>
    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "An error occurred while saving changes.");

            return Result.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Disposes the context asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the context.
    /// </summary>
    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}
