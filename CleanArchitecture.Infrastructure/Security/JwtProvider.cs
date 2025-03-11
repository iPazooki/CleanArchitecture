using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Infrastructure.Security;

internal sealed class JwtProvider(IOptions<JwtOptions> jwtOptions, IPermissionService permissionService) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        HashSet<string> permissions = await permissionService.GetPermissionsAsync(user.Id).ConfigureAwait(false);

        claims.AddRange(permissions.Select(permission => new Claim(Constants.Claims.Permissions, permission)));

        SigningCredentials signingCredentials =
            new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(_jwtOptions.Issuer, _jwtOptions.Audience, claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.TimeoutMinutes), signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
