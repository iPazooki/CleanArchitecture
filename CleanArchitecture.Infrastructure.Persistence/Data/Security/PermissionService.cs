using CleanArchitecture.Domain.Entities.Security;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Infrastructure.Persistence.Data.Security;

internal sealed class PermissionService(IApplicationUnitOfWork applicationUnitOfWork) : IPermissionService
{
    public async Task<HashSet<string>> GetPermissionsAsync(Guid memberId)
    {
        List<Role> roles = await applicationUnitOfWork.Users
            .Include(x => x.Roles)!
            .ThenInclude(x => x.Permissions)
            .Where(x => x.Id == memberId)
            .SelectMany(x => x.Roles!)
            .ToListAsync().ConfigureAwait(false);

        return roles
            .SelectMany(x => x.Permissions)
            .Select(x => x.Name)
            .ToHashSet();
    }
}
