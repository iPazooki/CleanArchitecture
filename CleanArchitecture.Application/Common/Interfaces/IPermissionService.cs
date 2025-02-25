namespace CleanArchitecture.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for a service that provides permissions for a member.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Asynchronously retrieves the set of permissions for a specified member.
    /// </summary>
    /// <param name="memberId">The unique identifier of the member.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a set of permissions.</returns>
    Task<HashSet<string>> GetPermissionsAsync(Guid memberId);
}