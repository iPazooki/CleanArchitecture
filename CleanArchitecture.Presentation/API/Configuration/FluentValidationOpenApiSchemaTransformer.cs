using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CleanArchitecture.Api.Configuration;

/// <summary>
/// Applies OpenAPI schema constraints that can be inferred from registered FluentValidation validators.
/// </summary>
internal sealed class FluentValidationOpenApiSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (schema.Properties is null)
        {
            return Task.CompletedTask;
        }

        Type targetType = context.JsonTypeInfo.Type;
        Type validatorType = typeof(IValidator<>).MakeGenericType(targetType);
        IEnumerable<IValidator> validators = context.ApplicationServices
            .GetServices(validatorType)
            .OfType<IValidator>();

        foreach (IValidator validator in validators)
        {
            ApplyValidatorRules(schema, context.JsonTypeInfo, targetType, validator);
        }

        return Task.CompletedTask;
    }

    private static void ApplyValidatorRules(OpenApiSchema schema, JsonTypeInfo jsonTypeInfo, Type targetType, IValidator validator)
    {
        IValidatorDescriptor descriptor = validator.CreateDescriptor();

        foreach (string memberName in descriptor.GetMembersWithValidators().Select(group => group.Key))
        {
            string? jsonPropertyName = ResolveJsonPropertyName(jsonTypeInfo, targetType, memberName);

            if (jsonPropertyName is null
                || !schema.Properties!.TryGetValue(jsonPropertyName, out IOpenApiSchema? propertySchema)
                || propertySchema is not OpenApiSchema concreteSchema)
            {
                continue;
            }

            ApplyPropertyRules(schema, descriptor, memberName, jsonPropertyName, concreteSchema);
        }
    }

    private static void ApplyPropertyRules(
        OpenApiSchema parentSchema,
        IValidatorDescriptor descriptor,
        string memberName,
        string jsonPropertyName,
        OpenApiSchema propertySchema)
    {
        foreach (IValidationRule rule in descriptor.GetRulesForMember(memberName))
        {
            foreach (IPropertyValidator? propertyValidator in rule.Components.Select(component => component.Validator))
            {
                string validatorName = propertyValidator.Name;

                if (validatorName is "NotEmptyValidator" or "NotNullValidator")
                {
                    parentSchema.Required ??= new HashSet<string>(StringComparer.Ordinal);
                    parentSchema.Required.Add(jsonPropertyName);
                }

                if (TryGetIntProperty(propertyValidator, "Min", out int minLength) && minLength != -1)
                {
                    propertySchema.MinLength = propertySchema.MinLength is null
                        ? minLength
                        : Math.Max(propertySchema.MinLength.Value, minLength);
                }

                if (TryGetIntProperty(propertyValidator, "Max", out int maxLength) && maxLength != -1)
                {
                    propertySchema.MaxLength = propertySchema.MaxLength is null
                        ? maxLength
                        : Math.Min(propertySchema.MaxLength.Value, maxLength);
                }

                if (validatorName is "RegularExpressionValidator" && TryGetStringProperty(propertyValidator, "Expression", out string? pattern) && pattern is not null)
                {
                    propertySchema.Pattern = pattern;
                }

                if (validatorName is "EmailValidator" or "AspNetCoreCompatibleEmailValidator")
                {
                    propertySchema.Format = "email";
                }
            }
        }
    }

    private static string? ResolveJsonPropertyName(JsonTypeInfo jsonTypeInfo, Type targetType, string memberName)
    {
        JsonPropertyInfo? jsonProperty = jsonTypeInfo.Properties.FirstOrDefault(property =>
            string.Equals(property.Name, memberName, StringComparison.Ordinal)
            || string.Equals(property.Name, JsonNamingPolicy.CamelCase.ConvertName(memberName), StringComparison.Ordinal));

        if (jsonProperty is not null)
        {
            return jsonProperty.Name;
        }

        PropertyInfo? propertyInfo = targetType.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (propertyInfo is null)
        {
            return null;
        }

        JsonPropertyNameAttribute? jsonPropertyNameAttribute = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
        return jsonPropertyNameAttribute?.Name ?? JsonNamingPolicy.CamelCase.ConvertName(propertyInfo.Name);
    }

    private static bool TryGetIntProperty(object validator, string propertyName, out int value)
    {
        PropertyInfo? property = validator.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        if (property?.GetValue(validator) is int intValue)
        {
            value = intValue;
            return true;
        }

        value = 0;
        return false;
    }

    private static bool TryGetStringProperty(object validator, string propertyName, out string? value)
    {
        PropertyInfo? property = validator.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        if (property?.GetValue(validator) is string stringValue)
        {
            value = stringValue;
            return true;
        }

        value = null;
        return false;
    }
}
