using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Security.Policies;

/// <summary>
/// Grants read access to any authenticated caller holding <b>at least one</b> of the four roles.
/// </summary>
/// <remarks>
/// The single <c>RequireRole</c> call below passes all four roles as alternatives, so the
/// requirement is satisfied by <b>ANY ONE</b> of them. This differs from
/// <see cref="EditorPolicy"/> and <see cref="AdminPolicy"/>, which chain separate
/// <c>RequireRole</c> calls and therefore demand <b>ALL</b> the listed roles.
/// </remarks>
internal sealed class ViewerPolicy : IAuthorizationPolicyDefinition
{
    public string Name => PolicyNames.Viewer;

    public void Configure(AuthorizationPolicyBuilder policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        policy.RequireAuthenticatedUser();

        // One call, four alternatives => the caller needs ANY ONE of these roles.
        policy.RequireRole(Roles.View, Roles.Create, Roles.Edit, Roles.Delete);
    }
}
