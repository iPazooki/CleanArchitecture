using DomainValidation;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Persistence.Data.UnitOfWork;

/// <summary>
/// Represents the unit of work for the application.
/// </summary>
/// <param name="context">The application database context.</param>
public partial class ApplicationUnitOfWork(ApplicationDbContext context, ILogger<ApplicationUnitOfWork> logger)
    : IApplicationUnitOfWork
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
            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while saving changes.");

            return Result.Failure(e.Message);
        }
    }

    /// <summary>
    /// Disposes the context asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
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