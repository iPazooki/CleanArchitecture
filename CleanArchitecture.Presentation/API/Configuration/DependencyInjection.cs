using CleanArchitecture.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi;

namespace CleanArchitecture.Api.Configuration;

internal static class DependencyInjection
{
    const string TestingEnvironment = "Testing";

    public static void AddPresentationServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddEndpointsApiExplorer();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();

        // Configure authentication based on environment
        if (builder.Environment.IsEnvironment(TestingEnvironment))
        {
            // Use test authentication handler for integration tests
            services.AddAuthentication(Authentication.TestAuthHandler.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, Authentication.TestAuthHandler>(
                    Authentication.TestAuthHandler.AuthenticationScheme,
                    options => { });
        }
        else
        {
            // Retrieve Keycloak configurations from builder.Configuration
            string validIssuers = builder.Configuration["Keycloak:ValidIssuers"]
                               ?? throw new InvalidOperationException("Keycloak ValidIssuers is not configured.");

            string realm = builder.Configuration["Keycloak:Realm"]
                               ?? throw new InvalidOperationException("Keycloak Realm is not configured.");

            string[] audience = builder.Configuration["Keycloak:Audience"]?.Split(",")
                                  ?? throw new InvalidOperationException("Keycloak Audience is not configured.");

            bool? requireHttpsMetadata = builder.Configuration.GetValue<bool?>("Keycloak:RequireHttpsMetadata");

            services.AddAuthentication()
                    .AddKeycloakJwtBearer(
                        serviceName: "keycloak",
                        realm: realm,
                        options =>
                        {
                            options.TokenValidationParameters.ValidAudiences = audience;
                            options.TokenValidationParameters.ValidIssuers = [validIssuers];
                            options.RequireHttpsMetadata = requireHttpsMetadata ?? !builder.Environment.IsDevelopment();
                        });
        }

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(ViewerPolicy.Name, ViewerPolicy.ConfigurePolicy)
            .AddPolicy(EditorPolicy.Name, EditorPolicy.ConfigurePolicy)
            .AddPolicy(AdminPolicy.Name, AdminPolicy.ConfigurePolicy);

        // Add API Versioning - URL segment only
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
        });

        // Store these for use in OpenApi configuration
        string? authorizationUrl = null;
        string? tokenUrl = null;

        if (!builder.Environment.IsEnvironment(TestingEnvironment))
        {
            authorizationUrl = builder.Configuration["Keycloak:AuthorizationUrl"]
                           ?? throw new InvalidOperationException("Keycloak AuthorizationUrl is not configured.");

            tokenUrl = builder.Configuration["Keycloak:TokenUrl"]
                   ?? throw new InvalidOperationException("Keycloak TokenUrl is not configured.");
        }

        services.AddOpenApi("v1", options =>
        {
            options.AddSchemaTransformer<FluentValidationOpenApiSchemaTransformer>();

            options.AddDocumentTransformer((document, context, ct) =>
            {
                document.Info.Title = "Clean Architecture API";
                document.Info.Version = "v1";

                // Replace {version} placeholder with actual version in all paths
                Dictionary<string, IOpenApiPathItem> updatedPaths = new();
                foreach (KeyValuePair<string, IOpenApiPathItem> path in document.Paths)
                {
                    string updatedPath = path.Key.Replace("{version}", "1", StringComparison.OrdinalIgnoreCase);
                    updatedPaths[updatedPath] = path.Value;
                }
                document.Paths = new OpenApiPaths();
                foreach (KeyValuePair<string, IOpenApiPathItem> path in updatedPaths)
                {
                    document.Paths.Add(path.Key, path.Value);
                }

                // Only add OAuth2 security scheme in non-Testing environments
                if (authorizationUrl is not null && tokenUrl is not null)
                {
                    // Ensure Components and SecuritySchemes are initialized
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

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
                }

                return Task.CompletedTask;
            });
        });
    }
}
