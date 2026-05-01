namespace CleanArchitecture.AppHost;

internal static class EndpointHelpers
{
    internal static string BuildExternalHttpsUrl(EndpointReference endpoint)
    {
        return $"https://{endpoint.Property(EndpointProperty.Host).ValueExpression}";
    }

    internal static string BuildInternalHttpUrl(EndpointReference endpoint)
    {
        return $"http://{endpoint.Property(EndpointProperty.Host).ValueExpression}:{endpoint.Property(EndpointProperty.Port).ValueExpression}";
    }
}
