using CleanArchitecture.Infrastructure.Security;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Api.OptionsSetup;

internal sealed class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtOptions>
{
    private const string JwtSection = "Jwt";

    public void Configure(JwtOptions options) => configuration.GetSection(JwtSection).Bind(options);
}
