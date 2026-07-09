using System.Text.Json.Serialization;

namespace CleanArchitecture.Infrastructure.Email;

/// <summary>
/// Source-generated serialization for the Brevo contracts. Brevo's wire format is camelCase.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(BrevoEmailRequest))]
[JsonSerializable(typeof(BrevoEmailResponse))]
[JsonSerializable(typeof(BrevoErrorResponse))]
internal sealed partial class BrevoJsonSerializerContext : JsonSerializerContext;
