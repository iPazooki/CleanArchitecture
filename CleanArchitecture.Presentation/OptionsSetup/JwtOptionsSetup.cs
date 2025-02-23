using CleanArchitecture.Infrastructure.Security;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Presentation.OptionsSetup;

internal class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtOptions>
{
    private const string JwtSection = "Jwt";
    
    public void Configure(JwtOptions options) => configuration.GetSection(JwtSection).Bind(options);
}