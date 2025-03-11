using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Security;

public sealed class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
{
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName).ConfigureAwait(false);

        return policy ?? new AuthorizationPolicyBuilder().AddRequirements(new PermissionRequirement(policyName)).Build();
    }
}
