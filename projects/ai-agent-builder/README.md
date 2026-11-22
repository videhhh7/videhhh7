# AI Agent Building Tool

An ASP.NET MVC portfolio project for creating configurable agents that call external REST services.

## Resume-aligned capabilities

- C# and ASP.NET MVC architecture.
- Reusable controllers, models and views.
- Configurable agent instructions and workflow definitions.
- REST API integration.
- JSON request/response processing.
- Separation between UI, orchestration and external-service adapters.

## Suggested structure

```text
src/
  AiAgentBuilder/
    Controllers/
    Models/
    Services/
    Views/
tests/
  AiAgentBuilder.Tests/
```

## Request flow

1. A user submits an agent configuration from the MVC view.
2. The controller validates the input model.
3. The orchestration service loads the configured workflow.
4. An API client sends a request to the selected external service.
5. The response is parsed into a typed result.
6. The controller returns a view model to the UI.

## Included starter code

- [Agent model](./src/Models/AgentDefinition.cs)
- [Agent service](./src/Services/AgentRunner.cs)

## Security and reliability

- Keep API keys in a secret store or environment variables, never in source control.
- Validate allowed API hosts to reduce SSRF risk.
- Apply request timeouts and cancellation tokens.
- Validate response status codes and JSON shape.
- Avoid forwarding arbitrary user instructions to privileged tools.
- Log correlation IDs and safe metadata, not secrets or sensitive payloads.

This is a starter implementation to extend with authentication, persistence, background execution and automated tests.
