using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Infrastructure.Security;

/// <summary>
/// Implements access to the current authenticated user's details using ASP.NET Core HttpContext.
/// </summary>
internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    /// <summary>
    /// Gets the username of the current authenticated user.
    /// </summary>
    public string? UserName => httpContextAccessor.HttpContext?.User?.Identity?.Name 
        ?? httpContextAccessor.HttpContext?.User?.FindFirstValue("preferred_username") 
        ?? httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
}
