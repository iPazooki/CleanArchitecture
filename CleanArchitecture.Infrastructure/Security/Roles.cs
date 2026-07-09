namespace CleanArchitecture.Infrastructure.Security;

/// <summary>
/// The role names granted by the identity provider (Keycloak realm roles or Entra app roles)
/// and consumed by the authorization policies.
/// </summary>
/// <remarks>
/// These values must stay in sync with the roles defined in the Keycloak realm export and
/// with the Entra application roles. Changing a value here silently revokes access.
/// </remarks>
public static class Roles
{
    /// <summary>Permits reading resources.</summary>
    public const string View = "view";

    /// <summary>Permits creating resources.</summary>
    public const string Create = "create";

    /// <summary>Permits modifying existing resources.</summary>
    public const string Edit = "edit";

    /// <summary>Permits removing resources.</summary>
    public const string Delete = "delete";

    /// <summary>Every role recognised by the application.</summary>
    public static IReadOnlyList<string> All { get; } = [View, Create, Edit, Delete];
}
