using CleanArchitecture.Infrastructure.Security;
using Microsoft.OpenApi;

namespace CleanArchitecture.Api.Configuration;

internal static class DependencyInjection
{
    public static void AddPresentationServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
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
                        options.TokenValidationParameters.ValidAudiences = ["scalar"];
                        options.TokenValidationParameters.ValidIssuers = [validIssuers];

                        // For development only - disable HTTPS metadata validation
                        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
                        {
                            options.RequireHttpsMetadata = false;
                        }
                    });

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

        services.AddOpenApi("v1", options =>
        {
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


                return Task.CompletedTask;
            });
        });
    }
}
