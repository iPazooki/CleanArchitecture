using CleanArchitecture.Infrastructure.Persistence.Data;
using Scalar.AspNetCore;

namespace CleanArchitecture.Api.Configuration;

internal static class WebApplicationExtensions
{
    /// <summary>
    /// Configures development-specific features such as OpenAPI UI, Scalar API reference,
    /// and ensures the database is created.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task ConfigureFeaturesAsync(this WebApplication app)
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
                .AddPreferredSecuritySchemes("OAuth2")
                .AddAuthorizationCodeFlow("OAuth2", flow =>
                {
                    flow.ClientId = "scalar";
                    flow.ClientSecret = clientSecret;
                    flow.Pkce = Pkce.Sha256;
                    flow.SelectedScopes = ["permissions"];
                }));
        }

        if (!app.Environment.IsProduction())
        {
            using IServiceScope scope = app.Services.CreateScope();
            ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync().ConfigureAwait(false);
        }

        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }
    }
}
