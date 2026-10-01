using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using Microsoft.Identity.Web;

namespace CleanArchitecture.Api.Configuration;

internal static class DependencyInjection
{
    public static void AddPresentationServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddEndpointsApiExplorer();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();

        string authProvider = builder.Configuration["Authentication:Provider"] ?? "Keycloak";

        if (authProvider.Equals("Entra", StringComparison.OrdinalIgnoreCase))
        {
            // Add Microsoft Entra ID Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
        }
        else
        {
            // Default to Keycloak authentication
            string validIssuers = builder.Configuration["Keycloak:ValidIssuers"]
                                  ?? throw new InvalidOperationException(
                                      "Keycloak ValidIssuers is not configured.");

            string realm = builder.Configuration["Keycloak:Realm"]
                           ?? throw new InvalidOperationException("Keycloak Realm is not configured.");

            string[] audience = builder.Configuration["Keycloak:Audience"]?.Split(",")
                                ?? throw new InvalidOperationException("Keycloak Audience is not configured.");

            bool? requireHttpsMetadata = builder.Configuration.GetValue<bool?>("Keycloak:RequireHttpsMetadata");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

        // Add API Versioning - URL segment only
        Asp.Versioning.IApiVersioningBuilder apiVersioning = services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        if (builder.Environment.IsDevelopment())
        {
            // Store these for use in OpenApi configuration
            string authorizationUrl;

            if (authProvider.Equals("Entra", StringComparison.OrdinalIgnoreCase))
            {
                authorizationUrl = builder.Configuration["ScalarApi:AuthorizationUrl"]
                                   ?? throw new InvalidOperationException("Scalar API AuthorizationUrl is not configured.");
            }
            else
            {
                // Default to Keycloak OpenApi settings
                authorizationUrl = builder.Configuration["Keycloak:AuthorizationUrl"]
                                   ?? throw new InvalidOperationException("Keycloak AuthorizationUrl is not configured.");
            }
            
            apiVersioning.AddOpenApi(versionedOptions =>
            {
                versionedOptions.Document.AddDocumentTransformer((document, _, _) =>
                {
                    string[] scopes = builder.Configuration["ScalarApi:Scopes"]?.Split(',') ?? [];

                    Dictionary<string, string> scopesDictionary = scopes.ToDictionary(scope => scope, scope => "API Access");

                    document.Info.Title = "Clean Architecture API";
                    document.Info.Version = "v1";
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                    document.Components.SecuritySchemes.Add("OAuth2",
                        new OpenApiSecurityScheme
                        {
                            Type =  SecuritySchemeType.OAuth2,
                            Description = "OAuth2 Implicit authentication",
                            Flows = new OpenApiOAuthFlows
                            {
                                Implicit = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new Uri(authorizationUrl),
                                    Scopes = scopesDictionary
                                }
                            }
                        });

                    return Task.CompletedTask;
                });
            });
        }
    }
}
