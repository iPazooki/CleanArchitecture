using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Security.Policies;

/// <summary>
/// Grants create and edit access to authenticated callers holding <b>all</b> of
/// <see cref="Roles.View"/>, <see cref="Roles.Create"/> and <see cref="Roles.Edit"/>.
/// </summary>
/// <remarks>
/// Each <c>RequireRole</c> call adds its own requirement, and every requirement must be
/// satisfied. A caller holding only <see cref="Roles.Edit"/> is therefore <b>denied</b>.
/// Contrast with <see cref="ViewerPolicy"/>, which needs only one role.
/// </remarks>
internal sealed class EditorPolicy : IAuthorizationPolicyDefinition
{
    public string Name => PolicyNames.Editor;

    public void Configure(AuthorizationPolicyBuilder policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        policy.RequireAuthenticatedUser();

        // Separate calls => the caller needs ALL of these roles.
        policy.RequireRole(Roles.View);
        policy.RequireRole(Roles.Create);
        policy.RequireRole(Roles.Edit);
    }
}
