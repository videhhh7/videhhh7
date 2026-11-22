# AI Agent Building Tool

ASP.NET Core MVC starter app for running named, configurable agents through HTTPS APIs.

## Run locally

Requires the .NET 8 SDK.

1. Copy `appsettings.example.json` to `appsettings.Development.json`.
2. Replace the example endpoint with an HTTPS endpoint you own or are authorized to call.
3. From this directory run:

```bash
dotnet restore
dotnet build
dotnet run
```

Open the local URL printed by ASP.NET Core.

## Design

- MVC controller handles form validation and response rendering.
- `IAgentEndpointRegistry` resolves a named endpoint from server-side configuration.
- `AgentRunner` sends JSON using an injected HttpClient and cancellation token.
- Endpoint values are not accepted as arbitrary URLs from the browser.

## Production hardening

Add authentication/authorization, per-user quotas, persistence, audit logging, response-size limits, outbound host allow-list enforcement, rate limiting and tests before deploying publicly. Do not store API keys in appsettings or commit secrets.
