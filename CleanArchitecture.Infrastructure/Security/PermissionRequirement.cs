using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Security;

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission => permission;
}
