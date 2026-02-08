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
    public static async Task UseDevelopmentFeaturesAsync(this WebApplication app)
    {
        if (!app.Environment.IsEnvironment("Testing"))
        {
            string? clientSecret = app.Configuration["ScalarApi:ClientSecret"];

            ArgumentNullException.ThrowIfNull(clientSecret);

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.MapScalarApiReference(options => options
                    .AddPreferredSecuritySchemes("OAuth2")
                    .AddAuthorizationCodeFlow("OAuth2", flow =>
                    {
                        flow.ClientId = "scalar";
                        flow.ClientSecret = clientSecret;
                        flow.Pkce = Pkce.Sha256;
                        flow.SelectedScopes = ["clean_api.all"];
                    }));

                // Initialize and create the database
                using IServiceScope scope = app.Services.CreateScope();
                ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await context.Database.EnsureCreatedAsync().ConfigureAwait(false);
            } 
        }
    }
}
