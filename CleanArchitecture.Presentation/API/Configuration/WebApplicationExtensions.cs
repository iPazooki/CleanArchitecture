using Scalar.AspNetCore;

namespace CleanArchitecture.Api.Configuration;

internal static class WebApplicationExtensions
{
    /// <summary>
    /// Configures development-specific features such as OpenAPI UI, Scalar API reference
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance.</param>
    public static void ConfigureEnvironments(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            string? clientSecret = app.Configuration["ScalarApi:ClientSecret"];
            string? clientId = app.Configuration["ScalarApi:ClientId"];
            string[]? scopes = app.Configuration["ScalarApi:Scopes"]?.Split(',');

            if (string.IsNullOrEmpty(clientSecret))
            {
                app.Logger.LogError(
                    "Scalar API client secret is not provided. " +
                    "It must be configured in Keycloak and stored in user secrets under 'ScalarApi:ClientSecret'. " +
                    "Skipping Scalar API reference registration.");
            }
            else if (string.IsNullOrEmpty(clientId))
            {
                app.Logger.LogError(
                    "Scalar API audience is not provided. " +
                    "It must be configured in Keycloak and stored in user secrets under 'ScalarApi:Audience'. " +
                    "Skipping Scalar API reference registration.");
            }
            else if (scopes is null)
            {
                app.Logger.LogError(
                    "Scalar API scopes are not provided. " +
                    "It must be configured in Keycloak and stored in user secrets under 'ScalarApi:Scopes'. " +
                    "Skipping Scalar API reference registration.");
            }
            else
            {
                app.MapOpenApi();

                app.MapScalarApiReference(options => options
                    .WithTitle("Clean Architecture API - v1")
                    .WithOpenApiRoutePattern("/openapi/{documentName}.json")
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                    .AddPreferredSecuritySchemes("OAuth2")
                    .AddAuthorizationCodeFlow("OAuth2", flow =>
                    {
                        flow.ClientId = clientId;
                        flow.ClientSecret = clientSecret;
                        flow.Pkce = Pkce.Sha256;
                        flow.SelectedScopes = scopes;
                    }));
            }
        }

        if (app.Environment.IsProduction())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }
    }
}
