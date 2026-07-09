using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Security.Policies;

/// <summary>
/// Grants full access to authenticated callers holding <b>all four</b> roles.
/// </summary>
/// <remarks>
/// Each <c>RequireRole</c> call adds its own requirement, and every requirement must be
/// satisfied. A caller holding <see cref="Roles.Delete"/> but not <see cref="Roles.View"/>
/// is therefore <b>denied</b>. Contrast with <see cref="ViewerPolicy"/>, which needs only one role.
/// </remarks>
internal sealed class AdminPolicy : IAuthorizationPolicyDefinition
{
    public string Name => PolicyNames.Admin;

    public void Configure(AuthorizationPolicyBuilder policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        policy.RequireAuthenticatedUser();

        // Separate calls => the caller needs ALL of these roles.
        policy.RequireRole(Roles.View);
        policy.RequireRole(Roles.Create);
        policy.RequireRole(Roles.Edit);
        policy.RequireRole(Roles.Delete);
    }
}
