using System.Net.Http.Json;
using AiAgentBuilder.Models;

namespace AiAgentBuilder.Services;

public sealed class AgentRunner(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<JsonDocument> RunAsync(
        AgentDefinition agent,
        string userInput,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(agent);

        if (string.IsNullOrWhiteSpace(agent.Name))
            throw new ArgumentException("Agent name is required.", nameof(agent));

        if (string.IsNullOrWhiteSpace(agent.EndpointName))
            throw new ArgumentException("A configured endpoint name is required.", nameof(agent));

        // EndpointName should resolve through a server-side allow-list/configuration.
        // Do not accept arbitrary URLs from an untrusted user.
        var payload = new
        {
            agent = agent.Name,
            instructions = agent.Instructions,
            input = userInput,
            parameters = agent.Parameters
        };

        using var response = await _httpClient.PostAsJsonAsync(
            agent.EndpointName,
            payload,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var document = await response.Content.ReadFromJsonAsync<JsonDocument>(
            cancellationToken: cancellationToken);

        return document ?? throw new InvalidOperationException(
            "The configured agent endpoint returned an empty JSON response.");
    }
}
