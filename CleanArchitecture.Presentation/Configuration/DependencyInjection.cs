using CleanArchitecture.Infrastructure.Security;
using CleanArchitecture.Presentation.OptionsSetup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
namespace CleanArchitecture.Presentation.Configuration;

internal static class DependencyInjection
{
    public static void AddPresentationServices(this IServiceCollection services)
    {
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        
        services.AddEndpointsApiExplorer();
        
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        services.AddProblemDetails();

        services.AddHealthChecks();
        services.AddOpenApi();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }
}
