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

            if (string.IsNullOrEmpty(clientSecret))
            {
                app.Logger.LogError(
                    "Scalar API client secret is not provided. " +
                    "It must be configured in Keycloak and stored in user secrets under 'ScalarApi:ClientSecret'. " +
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
                        flow.ClientId = "scalar";
                        flow.ClientSecret = clientSecret;
                        flow.Pkce = Pkce.Sha256;
                        flow.SelectedScopes = ["permissions"];
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
