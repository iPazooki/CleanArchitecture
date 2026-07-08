using System.Diagnostics.CodeAnalysis;

namespace CleanArchitecture.AppHost;

[SuppressMessage("Minor Vulnerability", "S5332:Clear-text protocols should not be used")]
internal static class EndpointHelpers
{
    internal static ReferenceExpression BuildExternalHttpsUrl(EndpointReference endpoint)
    {
        return ReferenceExpression.Create($"https://{endpoint.Property(EndpointProperty.Host)}");
    }

    internal static ReferenceExpression BuildInternalHttpUrl(EndpointReference endpoint)
    {
        return ReferenceExpression.Create($"http://{endpoint.Property(EndpointProperty.Host)}");
    }
}
