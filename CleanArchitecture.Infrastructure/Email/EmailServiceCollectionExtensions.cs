using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Infrastructure.Email;

/// <summary>
/// Registers the email provider.
/// </summary>
internal static class EmailServiceCollectionExtensions
{
    private const string ApiKeyHeaderName = "api-key";

    /// <summary>
    /// Registers <see cref="BrevoEmailService"/> as a resilient typed <see cref="HttpClient"/>,
    /// or <see cref="NullEmailService"/> when no Brevo API key is configured.
    /// </summary>
    /// <param name="services">The service collection to add the email provider to.</param>
    /// <param name="configuration">Configuration holding the <c>Brevo</c> section.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection(BrevoOptions.SectionName);

        // The Testing environment supplies no Brevo credentials. Binding BrevoOptions there would
        // fail ValidateOnStart and take the whole host down, so fall back to the null object.
        if (string.IsNullOrWhiteSpace(section[nameof(BrevoOptions.ApiKey)]))
        {
            services.AddSingleton<IEmailService, NullEmailService>();
            return services;
        }

        services.AddOptions<BrevoOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Resilience (Polly retry + circuit breaker + timeout, honouring Brevo's 429 Retry-After)
        // is supplied by ServiceDefaults' ConfigureHttpClientDefaults, which wraps every HTTP
        // client in the standard handler. Adding a second handler here would nest two pipelines
        // and multiply the retry count, so this client deliberately adds none.
        services.AddHttpClient<IEmailService, BrevoEmailService>(static (serviceProvider, client) =>
        {
            BrevoOptions options = serviceProvider.GetRequiredService<IOptions<BrevoOptions>>().Value;

            client.BaseAddress = new Uri(options.BaseAddress, UriKind.Absolute);
            client.DefaultRequestHeaders.Add(ApiKeyHeaderName, options.ApiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}
