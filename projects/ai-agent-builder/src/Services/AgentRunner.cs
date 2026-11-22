using System.Net.Http.Json;
using AiAgentBuilder.Models;

namespace AiAgentBuilder.Services;

public sealed class AgentRunner(HttpClient httpClient)
{
    public async Task<JsonDocument> RunAsync(
        AgentDefinition agent,
        Uri endpoint,
        string userInput,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(agent);
        ArgumentNullException.ThrowIfNull(endpoint);

        if (endpoint.Scheme != Uri.UriSchemeHttps)
            throw new InvalidOperationException("Agent endpoints must use HTTPS.");
        if (string.IsNullOrWhiteSpace(agent.Name))
            throw new ArgumentException("Agent name is required.", nameof(agent));

        var payload = new
        {
            agent = agent.Name,
            instructions = agent.Instructions,
            input = userInput,
            parameters = agent.Parameters
        };

        using var response = await httpClient.PostAsJsonAsync(endpoint, payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        var document = await response.Content.ReadFromJsonAsync<JsonDocument>(
            cancellationToken: cancellationToken);

        return document ?? throw new InvalidOperationException(
            "The configured agent endpoint returned an empty JSON response.");
    }
}
