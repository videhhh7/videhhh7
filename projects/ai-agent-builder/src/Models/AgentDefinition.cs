namespace AiAgentBuilder.Models;

public sealed class AgentDefinition
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    // Configure an allow-listed HTTPS endpoint in application settings.
    public string EndpointName { get; set; } = string.Empty;

    public Dictionary<string, string> Parameters { get; set; } = new();
}
