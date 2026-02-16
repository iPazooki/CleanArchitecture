using CleanArchitecture.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;

namespace CleanArchitecture.Api.Configuration;

internal static class DependencyInjection
{
    public static void AddPresentationServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddEndpointsApiExplorer();

        services.AddEndpointsApiExplorer();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();

        // Retrieve Keycloak configurations from builder.Configuration
        string validIssuers = builder.Configuration["Keycloak:ValidIssuers"]
                           ?? throw new InvalidOperationException("Keycloak ValidIssuers is not configured.");

        string authorizationUrl = builder.Configuration["Keycloak:AuthorizationUrl"]
                               ?? throw new InvalidOperationException("Keycloak AuthorizationUrl is not configured.");

        string tokenUrl = builder.Configuration["Keycloak:TokenUrl"]
                       ?? throw new InvalidOperationException("Keycloak TokenUrl is not configured.");

        services.AddAuthentication()
                .AddKeycloakJwtBearer(
                    serviceName: "keycloak",
                    realm: "clean-api",
                    options =>
                    {
                        options.TokenValidationParameters.ValidAudiences = ["scalar", "account"];
                        options.TokenValidationParameters.ValidIssuers = [validIssuers];

                        // For development only - disable HTTPS metadata validation
                        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
                        {
                            options.RequireHttpsMetadata = false;
                        }
                    });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, ct) =>
            {
                // Ensure Components and SecuritySchemes are initialized
                if (document.Components == null)
                {
                    document.Components = new OpenApiComponents();
                }
                if (document.Components.SecuritySchemes == null)
                {
                    document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>();
                }

                document.Components.SecuritySchemes.Add("OAuth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "OAuth2 authentication using Keycloak",
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(authorizationUrl),
                            TokenUrl = new Uri(tokenUrl),
                            Scopes = new Dictionary<string, string>
                            {
                                { "permissions", "Request for roles" }
                            }
                        }
                    }
                });


                return Task.CompletedTask;
            });
        });
    }
}
