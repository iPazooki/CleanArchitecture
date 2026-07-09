using System.Security.Claims;
using System.Text.Encodings.Web;
using CleanArchitecture.Infrastructure.Security;
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
        // Grant every role, so the principal also satisfies the policies that require all of them.
        List<Claim> claims =
        [
            new(ClaimTypes.Name, "TestUser"),
            new(ClaimTypes.NameIdentifier, "test-user-id")
        ];

        claims.AddRange(Roles.All.Select(role => new Claim(ClaimTypes.Role, role)));

        ClaimsIdentity identity = new(claims, AuthenticationScheme);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
