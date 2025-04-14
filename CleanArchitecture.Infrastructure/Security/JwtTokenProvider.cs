using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Infrastructure.Security;

internal sealed class JwtTokenProvider(IOptions<JwtOptions> jwtOptions, IPermissionService permissionService) : IJwtTokenProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<(string Token, DateTimeOffset ExpirationDate)> GenerateJwtTokenAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!)
        ];

        HashSet<string> permissions = await permissionService.GetPermissionsAsync(user.Id).ConfigureAwait(false);

        claims.AddRange(permissions.Select(permission => new Claim(Constants.Claims.Permissions, permission)));

        SigningCredentials signingCredentials = new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(_jwtOptions.Issuer, _jwtOptions.Audience, claims, expires: DateTime.UtcNow.AddMinutes(_jwtOptions.TokenValidityInMinutes), signingCredentials: signingCredentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
    }

    public (string Token, DateTimeOffset ExpirationDate) GenerateRefreshToken()
    {
        return (Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)), DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenValidityInDays));
    }
}
