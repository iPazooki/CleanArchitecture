using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Api.Authentication;

/// <summary>
/// Test authentication handler that automatically authenticates all requests
/// with the required roles for integration testing.
/// Only used when ASPNETCORE_ENVIRONMENT is set to "Testing".
/// </summary>
internal sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string AuthenticationScheme = "TestScheme";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Create claims with all required roles for testing
        Claim[] claims =
        [
            new(ClaimTypes.Name, "TestUser"),
            new(ClaimTypes.NameIdentifier, "test-user-id"),
            new(ClaimTypes.Role, "view"),
            new(ClaimTypes.Role, "create"),
            new(ClaimTypes.Role, "edit"),
            new(ClaimTypes.Role, "delete")
        ];

        ClaimsIdentity identity = new(claims, AuthenticationScheme);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
