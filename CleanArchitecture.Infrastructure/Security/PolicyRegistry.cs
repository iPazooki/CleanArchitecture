using CleanArchitecture.Infrastructure.Security.Policies;

namespace CleanArchitecture.Infrastructure.Security;

/// <summary>
/// The authorization policies the application registers at startup.
/// </summary>
/// <remarks>
/// Listed explicitly rather than discovered by assembly scanning: reflection-based discovery
/// is hostile to trimming and ahead-of-time compilation, and the set of policies is small
/// enough that an explicit list stays readable. To add a policy, add a file under
/// <c>Security/Policies</c> and one entry here.
/// </remarks>
internal static class PolicyRegistry
{
    public static IReadOnlyList<IAuthorizationPolicyDefinition> All { get; } =
    [
        new ViewerPolicy(),
        new EditorPolicy(),
        new AdminPolicy()
    ];
}
