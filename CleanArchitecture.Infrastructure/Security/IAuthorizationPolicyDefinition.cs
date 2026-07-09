using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Security;

/// <summary>
/// A single named authorization policy. Implementations are registered by
/// <c>AddSecurityServices</c> via <see cref="PolicyRegistry"/>, so adding a policy
/// requires no change to the API composition root.
/// </summary>
internal interface IAuthorizationPolicyDefinition
{
    /// <summary>The name the policy is registered under. See <see cref="PolicyNames"/>.</summary>
    string Name { get; }

    /// <summary>Applies the policy's requirements to <paramref name="policy"/>.</summary>
    void Configure(AuthorizationPolicyBuilder policy);
}
