namespace CleanArchitecture.Infrastructure.Security;

/// <summary>
/// The names under which the authorization policies are registered.
/// Endpoints reference these constants when calling <c>RequireAuthorization</c>.
/// </summary>
/// <remarks>
/// The literal values are load-bearing: they are the keys ASP.NET Core resolves policies by.
/// Renaming a constant is safe; changing its value is not.
/// </remarks>
public static class PolicyNames
{
    /// <summary>Name of the policy granting read access.</summary>
    public const string Viewer = "ViewerPolicy";

    /// <summary>Name of the policy granting read, create and edit access.</summary>
    public const string Editor = "EditorPolicy";

    /// <summary>Name of the policy granting full access.</summary>
    public const string Admin = "AdminPolicy";
}
