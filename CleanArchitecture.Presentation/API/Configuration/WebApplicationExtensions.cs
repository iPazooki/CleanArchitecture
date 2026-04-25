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
            string clientId = app.Configuration["ScalarApi:ClientId"] ??
                              throw new ArgumentException("Scalar API ClientId is not configured.");

            string[] scopes = app.Configuration["ScalarApi:Scopes"]?.Split(',') ??
                              throw new ArgumentException("Scalar API Scopes is not configured.");

            string authorizationUrl = app.Configuration["ScalarApi:AuthorizationUrl"] ??
                              throw new ArgumentException("Scalar API AuthorizationUrl is not configured.");

            app.MapOpenApi();

            app.MapScalarApiReference(options => options
                .WithTitle("Clean Architecture API - v1")
                .WithOpenApiRoutePattern("/openapi/{documentName}.json")
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .AddPreferredSecuritySchemes("OAuth2")
                .AddImplicitFlow("OAuth2", flow =>
                {
                    flow.AuthorizationUrl = authorizationUrl;
                    flow.ClientId = clientId;
                    flow.SelectedScopes = scopes;
                }));
        }

        if (app.Environment.IsProduction())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }
    }
}
