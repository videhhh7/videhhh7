namespace AiAgentBuilder.Services;

public interface IAgentEndpointRegistry
{
    Uri GetEndpoint(string endpointName);
}

public sealed class ConfigurationAgentEndpointRegistry(IConfiguration configuration)
    : IAgentEndpointRegistry
{
    public Uri GetEndpoint(string endpointName)
    {
        var value = configuration[$"AgentEndpoints:{endpointName}"];
        if (!Uri.TryCreate(value, UriKind.Absolute, out var endpoint) ||
            endpoint.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException(
                $"Endpoint '{endpointName}' is not configured as an absolute HTTPS URL.");
        }

        return endpoint;
    }
}
