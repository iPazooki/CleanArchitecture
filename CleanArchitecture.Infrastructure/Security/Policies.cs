using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Security;

public static class ViewerPolicy
{
    public const string Name = nameof(ViewerPolicy);

    public static void ConfigurePolicy(AuthorizationPolicyBuilder policy)
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole(Permissions.ViewRole, Permissions.CreateRole, Permissions.EditRole, Permissions.DeleteRole);
    }
}

public static class AdminPolicy
{
    public const string Name = nameof(AdminPolicy);

    public static void ConfigurePolicy(AuthorizationPolicyBuilder policy)
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole(Permissions.ViewRole);
        policy.RequireRole(Permissions.CreateRole);
        policy.RequireRole(Permissions.EditRole);
        policy.RequireRole(Permissions.DeleteRole);
    }
}

public static class EditorPolicy
{
    public const string Name = nameof(EditorPolicy);

    public static void ConfigurePolicy(AuthorizationPolicyBuilder policy)
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireRole(Permissions.ViewRole);
        policy.RequireRole(Permissions.CreateRole);
        policy.RequireRole(Permissions.EditRole);
    }
}
