using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CleanArchitecture.Domain.Entities.User;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Infrastructure.Security;

public sealed class JwtProvider(IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateJwtToken(User user)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        SigningCredentials signingCredentials = new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(_jwtOptions.Issuer, _jwtOptions.Audience, claims, expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}