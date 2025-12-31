using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authorization;
using CleanArchitecture.Infrastructure.Security;
using CleanArchitecture.Presentation.OptionsSetup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace CleanArchitecture.Presentation.Configuration;

internal static class DependencyInjection
{
    public static void AddPresentationServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Clean Architecture",
                Version = "v1"
            });
        });

        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddEndpointsApiExplorer();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();

        services.AddOpenApi();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }
}
