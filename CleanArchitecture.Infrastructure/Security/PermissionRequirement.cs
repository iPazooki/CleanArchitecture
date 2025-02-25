using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Security;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission => permission;
}